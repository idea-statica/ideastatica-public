using System.Text.Json.Serialization;

namespace IdeaStatiCa.PluginSystem.PluginList.Json
{
	internal class ExecutableDriver : Driver
	{
		[JsonPropertyName("path")]
		public string Path { get; set; } = string.Empty;

		[JsonPropertyName("additional_arguments")]
		public string[] AdditionalArguments { get; set; } = new string[0];

		public ExecutableDriver()
		{
			Type = "executable";
		}
	}
}