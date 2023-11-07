using IdeaStatiCa.CheckbotPlugin.Common;
using IdeaStatiCa.PluginSystem.PluginList.Descriptors;
using IdeaStatiCa.PluginSystem.PluginList.Json;
using IdeaStatiCa.PluginSystem.PluginList.Mappers;
using IdeaStatiCa.PluginSystem.PluginList.Storage;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IdeaStatiCa.PluginSystem.PluginList.Serialization
{
	internal class JsonPluginListSerializer
	{
		private readonly IStorage _storage;

		public JsonPluginListSerializer(IStorage storage)
		{
			_storage = storage;
		}

		public Task<List<PluginDescriptor>> Read()
		{
			return _storage.GetReadStream()
				.Map(ReadStream)
				.GetOrElse(Task.FromResult(new List<PluginDescriptor>()));
		}

		public async Task Write(IReadOnlyCollection<PluginDescriptor> pluginDescriptors)
		{
			using (Stream stream = _storage.GetWriteStream())
			{
				List<Plugin> plugins = pluginDescriptors.Select(Mapper.Map).ToList();
				await JsonSerializer.SerializeAsync(stream, plugins, GetOptions());
			}
		}

		private static Task<List<PluginDescriptor>> ReadStream(Stream stream)
		{
			using (StreamReader streamReader = new StreamReader(stream))
			{
				string content = streamReader.ReadToEnd();

				return Task.FromResult(JsonSerializer.Deserialize<List<Plugin>>(content, GetOptions())
					.ToMaybe()
					.Map(x => x.Select(Mapper.Map).ToList())
					.GetOrElse(new List<PluginDescriptor>()));
			}
		}

		private static JsonSerializerOptions GetOptions()
		{
			JsonSerializerOptions options = new JsonSerializerOptions()
			{
				WriteIndented = true,
				AllowTrailingCommas = true,
				PropertyNameCaseInsensitive = true
			};

			options.Converters.Add(new DriverConverter());
			options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

			return options;
		}
	}
}