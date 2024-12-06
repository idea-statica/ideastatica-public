using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace IdeaStatiCa.RcsApi
{
	public static class IdeaJsonSerializerSetting
	{
		public static JsonSerializerSettings GetJsonSettingIdea()
		{
			var settings = new JsonSerializerSettings();
			settings.Converters.Add(new StringEnumConverter { NamingStrategy = new CamelCaseNamingStrategy() });
			settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
			// Equivalent of PropertyNamingPolicy = CamelCase
			settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

			// Equivalent of PropertyNameCaseInsensitive = false
			settings.MetadataPropertyHandling = MetadataPropertyHandling.Default;

			// serialize type names for polymorphic types
			settings.TypeNameHandling = TypeNameHandling.Auto;

			return settings;
		}
	}
}
