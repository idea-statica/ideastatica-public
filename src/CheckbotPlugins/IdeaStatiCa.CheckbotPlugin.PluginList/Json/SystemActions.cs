using System.Text.Json.Serialization;

namespace IdeaStatiCa.PluginSystem.PluginList.Json
{
	internal class SystemActions
	{
		[JsonPropertyName("open")]
		public ActionButton? Open { get; set; }
	}
}