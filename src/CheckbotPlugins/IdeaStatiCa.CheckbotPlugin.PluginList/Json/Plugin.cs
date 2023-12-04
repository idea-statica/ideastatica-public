using System;
using System.Text.Json.Serialization;

namespace IdeaStatiCa.PluginSystem.PluginList.Json
{
	internal class Plugin
	{
		[JsonPropertyName("type")]
		public JsonPluginType Type { get; set; }

		[JsonPropertyName("driver")]
		public Driver Driver { get; set; } = new Driver();

		[JsonPropertyName("name")]
		public string Name { get; set; } = string.Empty;

		[JsonPropertyName("actions")]
		public SystemActions? Actions { get; set; }

		[JsonPropertyName("custom_actions")]
		public ActionButton[]? CustomActions { get; set; }
	}
}