using IdeaStatiCa.Plugin;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace SafFeaExampleLink
{
	/// <summary>
	/// Starts IDEA StatiCa Checkbot linked to an FEA application that saves its model as SAF files. A real link calls
	/// <see cref="Run"/> from a command of the FEA application.
	/// </summary>
	public static class CheckbotLink
	{
		/// <summary>
		/// Opens Checkbot and answers its requests until Checkbot is closed. The Checkbot project is kept in
		/// <paramref name="projectPath"/>, so the next run with the same folder opens it again.
		/// </summary>
		/// <param name="checkbotPath">Full path to IdeaCheckbot.exe of the installed IDEA StatiCa.</param>
		/// <param name="projectPath">
		/// Full path to a writable folder for the Checkbot project, one per FEA model, for example next to the model
		/// file. It is created if it does not exist.
		/// </param>
		/// <param name="safExport">The FEA application, which saves its model as SAF files.</param>
		/// <param name="logger">Receives the log of the link.</param>
		public static async Task Run(string checkbotPath, string projectPath, ISafExport safExport, IPluginLogger logger)
		{
			Directory.CreateDirectory(projectPath);
			logger.LogInformation($"Starting '{checkbotPath}' with the project in '{projectPath}'");

			try
			{
				var application = new SafApplication(projectPath, safExport, logger);
				IBIMPluginHosting hosting = new GrpcBimHostingFactory().Create(new PluginFactory(application, checkbotPath), logger);
				await hosting.RunAsync(Process.GetCurrentProcess().Id.ToString(), projectPath).ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				logger.LogError("The SAF FEA example link failed", ex);
				throw;
			}
		}

		/// <summary>
		/// Tells the hosting which Checkbot to start, and which object answers its requests.
		/// </summary>
		private sealed class PluginFactory : IBIMPluginFactory
		{
			private readonly IApplicationBIM _application;

			public PluginFactory(IApplicationBIM application, string checkbotPath)
			{
				_application = application;
				IdeaStaticaAppPath = checkbotPath;
			}

			public string FeaAppName => _application.GetApplicationName();

			public string IdeaStaticaAppPath { get; }

			public IApplicationBIM Create() => _application;
		}
	}
}
