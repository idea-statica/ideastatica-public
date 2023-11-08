using System.Text.Json.Serialization;

namespace IdeaStatiCa.PluginSystem.PluginList.Json
{
	internal class DotNetRunnerDriver : Driver
	{
		public const string TypeName = "dotnet_runner";

		[JsonPropertyName("path")]
		public string Path { get; set; } = string.Empty;

		[JsonPropertyName("class_name")]
		public string? ClassName { get; set; } = null;

		public DotNetRunnerDriver()
		{
			Type = TypeName;
		}
	}
}