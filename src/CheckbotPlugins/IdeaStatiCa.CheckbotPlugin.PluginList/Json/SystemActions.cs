using System.Text.Json.Serialization;

namespace IdeaStatiCa.CheckbotPlugin.PluginList.Json
{
	internal class SystemActions
	{
		[JsonPropertyName("open")]
		public OpenButton? Open { get; set; }
	}
}