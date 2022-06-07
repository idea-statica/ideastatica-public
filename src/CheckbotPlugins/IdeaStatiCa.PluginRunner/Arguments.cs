using CommandLine;

namespace IdeaStatiCa.PluginRunner
{
	internal class Arguments
	{
		[Option('p', "port", Required = true, HelpText = "Port of gRPC server.")]
		public int Port { get; set; }

		[Option('c', "classname", HelpText = "Name of the entrypoint class.")]
		public string? ClassName { get; set; }

		[Value(0, MetaName = "path", HelpText = "Path to assembly.")]
		public string Path { get; set; } = string.Empty;
	}
}