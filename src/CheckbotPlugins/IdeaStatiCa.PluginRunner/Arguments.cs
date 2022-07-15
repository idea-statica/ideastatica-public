using CommandLine;

namespace IdeaStatiCa.PluginRunner
{
	internal class Arguments
	{
		[Option("port", Required = true, HelpText = "Port of gRPC server.")]
		public int Port { get; set; }

		[Option("class-name", HelpText = "Name of the entrypoint class.")]
		public string? ClassName { get; set; }

		[Option("communication-id", Required = true, HelpText = "Communication ID.")]
		public string CommunicationId { get; set; } = string.Empty;

		[Option("path", Required = true, HelpText = "Path to assembly.")]
		public string Path { get; set; } = string.Empty;
	}
}