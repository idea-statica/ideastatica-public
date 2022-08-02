using IdeaStatiCa.BimApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace IdeaStatiCa.BimImporter.Persistence
{
	/// <summary>
	/// Stores id mappings and persistence tokens in a JSON file.
	/// </summary>
	public class JsonPersistence : AbstractPersistence, IFilePersistence
	{
		private static readonly PersistenceTokenConverter _tokenConverter = new PersistenceTokenConverter();

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
		/// Loads saved data from a JSON file.
		/// </summary>
		/// <param name="reader">TextReader to read saved info from.</param>
		public void Load(TextReader reader)
		{
			StoredData storedData = JsonConvert.DeserializeObject<StoredData>(reader.ReadToEnd(), _tokenConverter);

			Mappings = storedData.Mappings;
			Tokens = storedData.Tokens;

			DataLoaded?.Invoke();
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

			writer.Write(JsonConvert.SerializeObject(storedData, Formatting.Indented, _tokenConverter));
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