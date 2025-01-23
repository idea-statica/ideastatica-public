using Castle.Core.Internal;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace IdeaStatiCa.BimImporter.Persistence
{
	/// <summary>
	/// Stores id mappings and persistence tokens in a JSON file.
	/// </summary>
	public class JsonPersistence : AbstractPersistence, IFilePersistence
	{
		private readonly IPluginLogger _logger;

		private static readonly PersistenceTokenConverter _tokenConverter = new PersistenceTokenConverter();

		private JsonSerializerSettings GetJsonSerializerSettings()
		{
			return new JsonSerializerSettings()
			{
				Formatting = Formatting.Indented,
				Converters = new List<JsonConverter>() { _tokenConverter },
				TypeNameHandling = TypeNameHandling.Auto,
			};
		}

		/// <summary>
		/// Occurs when data are loaded.
		/// </summary>
		public override event Action DataLoaded;

		private sealed class StoredData
		{
			public HashSet<(int, string)> Mappings { get; set; }
			public Dictionary<string, IIdeaPersistenceToken> Tokens { get; set; }
		}

		/// <summary>
		/// Disabled default constructor
		/// </summary>
		private JsonPersistence()
		{
		}

		/// <summary>
		/// Creates an instance of the JsonPersistence class
		/// </summary>
		/// <param name="logger">The logger to be used for diagnostic messages</param>
		public JsonPersistence(IPluginLogger logger)
		{
			_logger = logger;
		}

		/// <summary>
		/// Loads saved data from a JSON file.
		/// </summary>
		/// <param name="reader">TextReader to read saved info from.</param>
		public void Load(TextReader reader)
		{
			// load the json string from the provider text reader
			string jsonStr = reader.ReadToEnd();

			// log the loaded data for the diagnostics purposes
			_logger.LogTrace($"Parsing id mappings json '{jsonStr}'.");

			try
			{
				AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

				// parse the json
				StoredData storedData = JsonConvert.DeserializeObject<StoredData>(jsonStr, GetJsonSerializerSettings());

				_logger.LogTrace($"Successfully parsed {storedData.Mappings.Count} mappings and {storedData.Tokens.Count} tokens.");

				Mappings = storedData.Mappings;
				Tokens = storedData.Tokens;

				DataLoaded?.Invoke();
			}
			catch (Exception ex)
			{
				// log the exception details
				_logger.LogInformation($"Parsing of id mappings json '{jsonStr}' failed", ex);

				// rethrow the original exception
				throw;
			}
			finally
			{
				AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
			}
		}

		private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			AppDomain currentDomain = AppDomain.CurrentDomain;
			//Provide the current application domain evidence for the assembly.

			//Make an array for the list of assemblies.
			Assembly[] assems = currentDomain.GetAssemblies();

			var foundAssembly = Array.Find(assems, asem => (asem.ManifestModule.Name == args.Name || asem.ManifestModule.Name == args.Name + ".dll"));
			//for framework 4.8 app it trying load System.Core assembly and it cause crash. in assembly list its System.Core.dll 
			if (foundAssembly == null)
			{
				string curAssPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
				return Assembly.LoadFrom(Path.Combine(curAssPath,args.Name + ".dll"));
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Loads saved data from a JSON stream.
		/// </summary>
		/// <param name="stream">Stream to read saved info from.</param>
		public void Load(Stream stream)
		{
			using (StreamReader streamReader = new StreamReader(stream))
			{
				Load(streamReader);
			}
		}

		/// <summary>
		/// Saves stored data in a JSON file.
		/// </summary>
		/// <param name="writer">TextWriter to write stored info into.</param>
		public void Save(TextWriter writer)
		{
			StoredData storedData = new StoredData()
			{
				Mappings = Mappings,
				Tokens = Tokens
			};

			writer.Write(JsonConvert.SerializeObject(storedData, GetJsonSerializerSettings()));
		}

		/// <summary>
		/// Saves stored data in a JSON file.
		/// </summary>
		/// <param name="stream">Stream to write stored info into.</param>
		public void Save(Stream stream)
		{
			using (StreamWriter streamWriter = new StreamWriter(stream))
			{
				Save(streamWriter);
			}
		}
	}
}