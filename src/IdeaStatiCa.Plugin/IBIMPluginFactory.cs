using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin
{
	public interface IBIMPluginHosting
	{
		Task RunAsync(string id, string workingDirectory);

		event ISEventHandler AppStatusChanged;

		IApplicationBIM Service { get; }
	}

	public interface IBIMPluginFactory
	{
		IApplicationBIM Create();

		string FeaAppName { get; }

		string IdeaStaticaAppPath { get; }
	}
}
