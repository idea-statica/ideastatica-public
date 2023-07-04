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
	}
}
