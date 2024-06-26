using System.Text.Json.Serialization;

namespace IdeaStatiCa.CheckbotPlugin.PluginList.Json
{
	internal class TabInfo
	{
		[JsonPropertyName("create_separate_tab")]
		public bool CreateSeparateTab { get; set; } = false;

		[JsonPropertyName("tab_name")]
		public string TabName { get; set; } = "Plugins";
	}
}
