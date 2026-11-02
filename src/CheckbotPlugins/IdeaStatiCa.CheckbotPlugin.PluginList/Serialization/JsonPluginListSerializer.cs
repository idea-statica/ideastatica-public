using IdeaStatiCa.CheckbotPlugin.PluginList.Descriptors;
using IdeaStatiCa.CheckbotPlugin.PluginList.Json;
using IdeaStatiCa.CheckbotPlugin.PluginList.Mappers;
using IdeaStatiCa.CheckbotPlugin.PluginList.Storage;
using IdeaStatiCa.CheckbotPlugin.PluginList.Utils;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IdeaStatiCa.CheckbotPlugin.PluginList.Serialization
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
				.ToMaybe()
				.Map(ReadStream)
				.GetOrElse(Task.FromResult(new List<PluginDescriptor>()));
		}

		public async Task Write(IReadOnlyCollection<PluginDescriptor> pluginDescriptors)
		{
			using Stream stream = _storage.GetWriteStream();
			List<Plugin> plugins = pluginDescriptors.Select(Mapper.Map).ToList();
			await JsonSerializer.SerializeAsync(stream, plugins, GetOptions());
		}

		private static Task<List<PluginDescriptor>> ReadStream(Stream stream)
		{
			using StreamReader streamReader = new(stream);
			string content = streamReader.ReadToEnd();

			return Task.FromResult(JsonSerializer.Deserialize<List<Plugin>>(content, GetOptions())
				.ToMaybe()
				.Map(x => x.Select(Mapper.Map).ToList())
				.GetOrElse([]));
		}

		private static JsonSerializerOptions GetOptions()
		{
			JsonSerializerOptions options = new()
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