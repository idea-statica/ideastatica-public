using Newtonsoft.Json;

namespace ConnectionWebClient.Tools
{
	public static class JsonTools
	{
		public static string ToFormatedJson(object instance)
		{
			if(instance == null)
			{
				return string.Empty;
			}

			string json = JsonConvert.SerializeObject(instance, Formatting.Indented);
			return json;
		}

		public static string FormatJson(string json)
		{
			object? instance = JsonConvert.DeserializeObject(json);
			if(instance == null)
			{
				return string.Empty;
			}

			dynamic parsedJson = instance;
			return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
		}
	}
}
