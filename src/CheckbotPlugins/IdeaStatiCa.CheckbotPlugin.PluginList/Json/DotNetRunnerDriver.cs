using System.Text.Json.Serialization;

namespace IdeaStatiCa.PluginSystem.PluginList.Json
{
	internal class DotNetRunnerDriver : Driver
	{
		[JsonPropertyName("path")]
		public string Path { get; set; } = string.Empty;

		[JsonPropertyName("class_name")]
		public string ClassName { get; set; } = string.Empty;

		public DotNetRunnerDriver()
		{
			Type = "dotnet_runner";
		}
	}
}