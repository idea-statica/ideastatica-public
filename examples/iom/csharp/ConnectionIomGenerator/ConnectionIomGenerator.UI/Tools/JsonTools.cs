using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
	}
}
