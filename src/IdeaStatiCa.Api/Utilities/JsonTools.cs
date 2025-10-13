using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace IdeaStatiCa.Api.Utilities
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
			settings.Converters.Add(new StringEnumConverter { NamingStrategy = new CamelCaseNamingStrategy() });
			settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
			// Equivalent of PropertyNamingPolicy = CamelCase
			settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

			// Equivalent of PropertyNameCaseInsensitive = false
			settings.MetadataPropertyHandling = MetadataPropertyHandling.Default;

			// serialize type names for polymorphic types
			settings.TypeNameHandling = TypeNameHandling.Auto;
		}

		// method to serialize data suitable for npm package https://www.npmjs.com/package/@ideastatica/scene
		public static void SetFor3DScene(this JsonSerializerSettings settings)
		{
			settings.SetForIdea();
			// Settings required for proper 3D Scene data to process with npm package
			settings.NullValueHandling = NullValueHandling.Ignore;
			settings.TypeNameHandling = TypeNameHandling.None;
		}
	}
}
