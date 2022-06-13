using CommandLine;

namespace IdeaStatiCa.PluginRunner
{
	internal class Arguments
	{
		[Option('p', "port", Required = true, HelpText = "Port of gRPC server.")]
		public int Port { get; set; }

		[Option('c', "classname", HelpText = "Name of the entrypoint class.")]
		public string? ClassName { get; set; }

		[Option('i', "id", Required = true, HelpText = "Communication ID.")]
		public string CommunicationId { get; set; } = string.Empty;

		[Value(0, MetaName = "path", HelpText = "Path to assembly.")]
		public string Path { get; set; } = string.Empty;
	}
}