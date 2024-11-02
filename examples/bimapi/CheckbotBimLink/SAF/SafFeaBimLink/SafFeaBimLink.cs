using IdeaStatiCa.Plugin;
using IdeaStatiCa.PluginLogger;
using System.Diagnostics;

namespace SafFeaBimLink
{
	public class SafFeaBimLinkApp
	{
		private static readonly IPluginLogger logger;

		static SafFeaBimLinkApp()
		{
			SerilogFacade.Initialize();
			logger = LoggerProvider.GetLogger("SafFeaBimLinkApp");
		}

		public static async Task Run(ISafDataSource safDataSource)
		{


			try
			{
				logger.LogInformation("Saf Fea Link started");

				string workingDirectory = Path.Combine(safDataSource.GetModelDirectory(), "IdeaStatiCa-" + safDataSource.GetModelName());
				if (!Directory.Exists(workingDirectory))
				{
					Directory.CreateDirectory(workingDirectory);
				}

				var bimHosting = new GrpcBimHostingFactory();
				PluginFactory pluginFactory = new PluginFactory(logger, safDataSource, workingDirectory, bimHosting.InitGrpcClient(logger));
				IBIMPluginHosting pluginHosting = bimHosting.Create(pluginFactory, logger);

				logger.LogDebug("Starting Checkbot");

				//Run GRPC
				await pluginHosting.RunAsync(Process.GetCurrentProcess().Id.ToString(), pluginFactory.WorkingDirectory);
			}
			catch (Exception e)
			{
				logger.LogError("Saf Fea Link Crashed", e);
			}
			finally
			{
				logger.LogInformation("Saf Fea Link finished");
			}
		}
	}
}
