using System.Text.Json.Serialization;

namespace IdeaStatiCa.CheckbotPlugin.PluginList.Json
{
	internal class ActionButton
	{
		[JsonPropertyName("name")]
		public string Name { get; set; } = string.Empty;

		[JsonPropertyName("image")]
		public string Image { get; set; } = string.Empty;

		[JsonPropertyName("text")]
		public string Text { get; set; } = string.Empty;

		[JsonPropertyName("tooltip")]
		public string Tooltip { get; set; } = string.Empty;

		[JsonPropertyName("tooltip_disabled")]
		public string TooltipDisabled { get; set; } = string.Empty;

		[JsonPropertyName("tooltip_link")]
		public string? TooltipLink { get; set; } = string.Empty;
	}
}