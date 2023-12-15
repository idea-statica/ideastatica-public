using Newtonsoft.Json.Linq;

namespace RcsApiClient
{
	internal static class Tools
	{
		internal static string FormatJson(string json)
		{
			JToken parsedJson = JToken.Parse(json);
			return parsedJson.ToString(Newtonsoft.Json.Formatting.Indented);

		}
	}
}
