using System.Text.Json.Serialization;

namespace IdeaStatiCa.CheckbotPlugin.PluginList.Json
{
	internal class TabInfo
	{
		private static readonly string _defaultTabName = "Plugins";

		[JsonPropertyName("tab_name")]
		public string TabName { get; set; } = _defaultTabName;
	}
}
