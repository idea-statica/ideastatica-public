using System;
using System.Text.Json.Serialization;

namespace IdeaStatiCa.CheckbotPlugin.PluginList.Json
{
	internal class ExecutableDriver : Driver
	{
		public const string TypeValue = "executable";

		[JsonPropertyName("path")]
		public string Path { get; set; } = string.Empty;

		[JsonPropertyName("additional_arguments")]
		public string[] AdditionalArguments { get; set; } = Array.Empty<string>();

		public ExecutableDriver()
		{
			Type = TypeValue;
		}
	}
}