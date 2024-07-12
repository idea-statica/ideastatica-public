using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace IdeaStatiCa.Plugin.Utilities
{
	public static class JsonTools
	{
		public static JsonSerializerSettings CreateIdeaRestJsonSettings()
		{
			JsonSerializerSettings settings = new JsonSerializerSettings();
			settings.SetForIdea();
			return settings;
		}

		public static void SetForIdea(this JsonSerializerSettings settings)
		{

			settings.Converters.Add(new StringEnumConverter());
			settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
			// Equivalent of PropertyNamingPolicy = CamelCase
			settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

			// Equivalent of PropertyNameCaseInsensitive = false
			settings.MetadataPropertyHandling = MetadataPropertyHandling.Default;

			// serialize type names for polymorphic types
			settings.TypeNameHandling = TypeNameHandling.Auto;

		}
	}
}
