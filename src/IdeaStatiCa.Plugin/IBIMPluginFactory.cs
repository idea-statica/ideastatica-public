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

	/// <summary>
	/// A factory that also wants extra arguments passed to the IDEA StatiCa application it names. Implement this
	/// alongside <see cref="IBIMPluginFactory"/> when the link has more to say than the project directory — for
	/// example a link that collected the project's design codes up front and needs the application to create it
	/// (<c>-new -designCode:…</c>).
	/// </summary>
	/// <remarks>
	/// Deliberately a separate interface rather than another member on <see cref="IBIMPluginFactory"/>: that one is
	/// implemented by links built outside this repository, and adding a member would break every one of them. A
	/// factory that does not implement this is passed the same command line as before.
	/// </remarks>
	public interface IBIMPluginFactoryWithArguments : IBIMPluginFactory
	{
		/// <summary>
		/// Arguments appended to the ones the host always passes (<c>-automation</c>, <c>-project</c>,
		/// <c>-grpcPort</c>). Null or empty changes nothing.
		/// </summary>
		string IdeaStaticaAppArguments { get; }
	}
}
