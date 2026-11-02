using System.Text.Json.Serialization;

namespace IdeaStatiCa.CheckbotPlugin.PluginList.Json
{
	internal class Driver
	{
		[JsonPropertyName("type")]
		public string Type { get; set; } = string.Empty;
	}
}