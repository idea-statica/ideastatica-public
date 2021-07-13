using IdeaStatiCa.BimApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace IdeaStatiCa.BimImporter.Persistence
{
	/// <summary>
	/// A converter for serialization and deserialization of <see cref="IIdeaPersistenceToken"/> objects.
	/// </summary>
	internal class PersistenceTokenConverter : JsonConverter
	{
		private const string DataName = "$data";
		private const string TypeName = "$type";

		public override bool CanConvert(Type objectType)
		{
			return typeof(IIdeaPersistenceToken).IsAssignableFrom(objectType);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			JObject obj = JObject.Load(reader);
			Type type = Type.GetType(obj.GetValue(TypeName).Value<string>());

			if (type is null)
			{
				throw new InvalidOperationException();
			}

			return obj.GetValue(DataName).ToObject(type);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			writer.WriteStartObject();

			writer.WritePropertyName(TypeName);
			writer.WriteValue(value.GetType().AssemblyQualifiedName);

			writer.WritePropertyName(DataName);
			JObject.FromObject(value).WriteTo(writer);

			writer.WriteEndObject();
		}
	}
}