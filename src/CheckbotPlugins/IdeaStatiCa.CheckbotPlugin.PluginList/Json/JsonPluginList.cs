using AutoMapper;
using IdeaStatiCa.CheckbotPlugin.Common;
using IdeaStatiCa.PluginSystem.PluginList.Descriptors;
using IdeaStatiCa.PluginSystem.PluginList.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IdeaStatiCa.PluginSystem.PluginList.Json
{
	internal class JsonPluginList
	{
		internal static readonly IMapper Mapper
			= new Mapper(new MapperConfiguration(
				  x => x.AddProfile(typeof(JsonMappingProfile))));

		private readonly IStorage _storage;

		public JsonPluginList(IStorage storage)
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
				List<Plugin> plugins = Mapper.Map<List<Plugin>>(pluginDescriptors);
				return JsonSerializer.SerializeAsync(stream, plugins, GetOptions());
			}
		}

		private static async Task<List<PluginDescriptor>> ReadStream(Stream stream)
		{
			using (stream)
			{
				return Maybe.From(await JsonSerializer.DeserializeAsync<List<Plugin>>(stream, GetOptions()))
					.Map(x => Mapper.Map<List<PluginDescriptor>>(x))
					.Get(new List<PluginDescriptor>());
			}
		}

		private static JsonSerializerOptions GetOptions()
		{
			JsonSerializerOptions options = new JsonSerializerOptions()
			{
				WriteIndented = true,
				AllowTrailingCommas = true
			};

			options.Converters.Add(new DriverConverter());

			return options;
		}

		private sealed class DriverConverter : JsonConverter<Driver>
		{
			public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(Driver);

			public override Driver Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			{
				JsonDocument doc = JsonDocument.ParseValue(ref reader);

				return Maybe.From(doc.RootElement.GetProperty("type").GetString())
					.Bind(x => DeserializeDriver(x, doc, options))
					.Get(new Driver());
			}

			public override void Write(Utf8JsonWriter writer, Driver value, JsonSerializerOptions options)
			{
				JsonSerializer.Serialize(writer, value, value.GetType(), options);
			}

			private static Maybe<Driver> DeserializeDriver(string type, JsonDocument doc, JsonSerializerOptions options)
			{
				switch (type)
				{
					case "dotnet_runner":
						return Maybe.From<Driver>(JsonSerializer.Deserialize<DotNetRunnerDriver>(doc.RootElement.GetRawText(), options));

					case "executable":
						return Maybe.From<Driver>(JsonSerializer.Deserialize<ExecutableDriver>(doc.RootElement.GetRawText(), options));
				}

				return new Maybe<Driver>();
			}
		}
	}
}