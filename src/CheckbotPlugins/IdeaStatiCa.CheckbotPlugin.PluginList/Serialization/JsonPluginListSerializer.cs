using AutoMapper;
using IdeaStatiCa.CheckbotPlugin.Common;
using IdeaStatiCa.PluginSystem.PluginList.Descriptors;
using IdeaStatiCa.PluginSystem.PluginList.Json;
using IdeaStatiCa.PluginSystem.PluginList.Storage;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IdeaStatiCa.CheckbotPlugin.PluginList.Serialization
{
	internal class JsonPluginListSerializer
	{
		internal static readonly IMapper _mapper
			= new Mapper(new MapperConfiguration(
				  x => x.AddProfile(typeof(JsonMappingProfile))));

		private readonly IStorage _storage;

		public JsonPluginListSerializer(IStorage storage)
		{
			_storage = storage;
		}

		public Task<List<PluginDescriptor>> Read()
		{
			return _storage.GetReadStream()
				.Map(ReadStream)
				.Get(Task.FromResult(new List<PluginDescriptor>()));
		}

		public Task Write(IReadOnlyCollection<PluginDescriptor> pluginDescriptors)
		{
			using (Stream stream = _storage.GetWriteStream())
			{
				List<Plugin> plugins = _mapper.Map<List<Plugin>>(pluginDescriptors);
				return JsonSerializer.SerializeAsync(stream, plugins, GetOptions());
			}
		}

		private static async Task<List<PluginDescriptor>> ReadStream(Stream stream)
		{
			using (stream)
			{
				return Maybe.From(await JsonSerializer.DeserializeAsync<List<Plugin>>(stream, GetOptions()))
					.Map(x => _mapper.Map<List<PluginDescriptor>>(x))
					.Get(new List<PluginDescriptor>());
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