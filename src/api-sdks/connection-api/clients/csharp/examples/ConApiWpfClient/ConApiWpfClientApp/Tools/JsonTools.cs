using Newtonsoft.Json;

namespace ConApiWpfClientApp.Tools
{
	/// <summary>
	/// Utility methods for JSON serialization and formatting.
	/// </summary>
	public static class JsonTools
	{
		/// <summary>
		/// Serializes an object to a formatted (indented) JSON string.
		/// </summary>
		/// <param name="instance">The object to serialize.</param>
		/// <returns>A formatted JSON string, or <see cref="string.Empty"/> if the instance is <see langword="null"/>.</returns>
		public static string ToFormatedJson(object instance)
		{
			if(instance == null)
			{
				return string.Empty;
			}

			string json = JsonConvert.SerializeObject(instance, Formatting.Indented);
			return json;
		}

		/// <summary>
		/// Reformats a JSON string with indentation.
		/// </summary>
		/// <param name="json">The JSON string to format.</param>
		/// <returns>The formatted JSON string, or <see cref="string.Empty"/> if deserialization fails.</returns>
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
