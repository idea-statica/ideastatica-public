using Newtonsoft.Json;
using System.Globalization;

namespace ConnectionIomGenerator.UI.Tools
{
	internal static class JsonTools
	{
		private readonly static JsonSerializerSettings _jsonSettings;

		static JsonTools()
		{
			_jsonSettings = new JsonSerializerSettings()
			{
				Culture = CultureInfo.InvariantCulture,
				TypeNameHandling = TypeNameHandling.None

			};
		}

		internal static string GetJsonText<T>(T obj)
		{
			var defaultConversionsJson = JsonConvert.SerializeObject(obj, Formatting.Indented, _jsonSettings);
			return defaultConversionsJson;
		}

		internal static T? DeserializeJson<T>(string jsonText)
		{
			T? res = JsonConvert.DeserializeObject<T>(jsonText, _jsonSettings);
			return res;
		}
	}
}
