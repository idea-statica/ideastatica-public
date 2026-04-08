using IdeaStatiCa.BimApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace IdeaStatiCa.BimImporter.Persistence
{
	/// <summary>
	/// A converter for serialization and deserialization of <see cref="IIdeaPersistenceToken"/> objects.
	/// </summary>
	internal class PersistenceTokenConverter : JsonConverter
	{
		private const string DataName = "$data";
		private const string TypeName = "$type";
		private JsonSerializer jsonSerializer;

		private JsonSerializer GetJsonSerializer()
		{
			return jsonSerializer ??
					(
						jsonSerializer = JsonSerializer.Create(new JsonSerializerSettings
						{
							TypeNameHandling = TypeNameHandling.All,
							TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
							Converters = new List<JsonConverter> { new TypeJsonConverter() }
						}
					)
				);
		}

		/// <summary>
		/// Handles serialization of <see cref="System.Type"/> properties using version-tolerant type resolution.
		/// Newtonsoft's default Type converter uses Type.GetType which requires an exact version match.
		/// </summary>
		private sealed class TypeJsonConverter : JsonConverter<Type>
		{
			public override Type ReadJson(JsonReader reader, Type objectType, Type existingValue, bool hasExistingValue, JsonSerializer serializer)
			{
				if (reader.TokenType == JsonToken.Null)
				{
					return null;
				}

				string typeStr = (string)reader.Value;
				return Type.GetType(typeStr, AssemblyResolver, TypeResolver, false, false);
			}

			public override void WriteJson(JsonWriter writer, Type value, JsonSerializer serializer)
			{
				writer.WriteValue(value?.AssemblyQualifiedName);
			}
		}

		public override bool CanConvert(Type objectType)
		{
			return typeof(IIdeaPersistenceToken).IsAssignableFrom(objectType);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			// load the json object
			JObject obj = JObject.Load(reader);

			// get the value of the "$type" property
			string typeStr = obj.GetValue(TypeName).Value<string>();

			// get the type
			Type type = Type.GetType(
				typeStr,
				AssemblyResolver,
				TypeResolver,
				false,
				false);

			// check that the type was successfully created from the loaded json object
			if (type is null)
			{
				throw new InvalidOperationException($"Type name '{typeStr}' loaded from json could not be converted to a type.");
			}

			// get the object "$data" in the type of "$type"
			return obj.GetValue(DataName).ToObject(type, GetJsonSerializer());
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			writer.WriteStartObject();

			writer.WritePropertyName(TypeName);
			writer.WriteValue(value.GetType().AssemblyQualifiedName);

			writer.WritePropertyName(DataName);
			JObject.FromObject(value, GetJsonSerializer()).WriteTo(
				writer, new JsonConverter[] { this });

			writer.WriteEndObject();
		}

		private static Assembly AssemblyResolver(AssemblyName assemblyName)
		{
			// Try to find already-loaded assembly by simple name, ignoring version differences
			// (handles cases where persisted JSON references an older version of the assembly)
			foreach (Assembly loaded in AppDomain.CurrentDomain.GetAssemblies())
			{
				if (string.Equals(loaded.GetName().Name, assemblyName.Name, StringComparison.OrdinalIgnoreCase))
				{
					return loaded;
				}
			}

			return Assembly.Load(assemblyName);
		}

		private static Type TypeResolver(Assembly assembly, string name, bool ignoreCase)
		{
			if (name.StartsWith("IdeaStatica.", StringComparison.OrdinalIgnoreCase))
			{
				return assembly.GetType(name, false, true);
			}

			return assembly.GetType(name, false, ignoreCase);
		}
	}
}