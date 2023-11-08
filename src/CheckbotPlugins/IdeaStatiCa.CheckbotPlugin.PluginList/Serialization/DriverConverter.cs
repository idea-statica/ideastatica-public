using IdeaStatiCa.CheckbotPlugin.Common;
using IdeaStatiCa.PluginSystem.PluginList.Json;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IdeaStatiCa.PluginSystem.PluginList.Serialization
{
	internal class DriverConverter : JsonConverter<Driver>
	{
		public override Driver Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			JsonDocument doc = JsonDocument.ParseValue(ref reader);

			return doc.RootElement.GetProperty("type").GetString()
				.ToMaybe()
				.Bind(x => DeserializeDriver(x!, doc, options))
				.GetOrElse(new Driver());
		}

		public override void Write(Utf8JsonWriter writer, Driver value, JsonSerializerOptions options)
		{
			JsonSerializer.Serialize(writer, value, value.GetType(), options);
		}

		private static Maybe<Driver> DeserializeDriver(string type, JsonDocument doc, JsonSerializerOptions options)
		{
			switch (type.ToLower())
			{
				case DotNetRunnerDriver.TypeName:
					return JsonSerializer.Deserialize<DotNetRunnerDriver>(doc.RootElement.GetRawText(), options)!.ToMaybe<Driver>();

				case ExecutableDriver.TypeValue:
					return JsonSerializer.Deserialize<ExecutableDriver>(doc.RootElement.GetRawText(), options)!.ToMaybe<Driver>();
			}

			return Maybe<Driver>.Empty();
		}
	}
}