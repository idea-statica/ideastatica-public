using SafFeaApi_MOCK;
using IdeaStatiCa.Diagnostics;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.PluginLogger;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.IO;
using System;

namespace SafFeaBimLink
{
    public class SafFeaBimLinkApp
    {
		public static async  Task Run(FeaModelApiClient feaModelApi)
        {
			SerilogFacade.Initialize();
			IPluginLogger logger = LoggerProvider.GetLogger("saffeappexample");

			try
            {
				logger.LogInformation("Saf Fea Link started");

                string workingDirectory = Path.Combine(Path.GetDirectoryName(feaModelApi.GetModelFilePath()), "IdeaStatiCa-" + feaModelApi.GetModelName());
                if (!Directory.Exists(workingDirectory))
                {
                    Directory.CreateDirectory(workingDirectory);
                }

				var bimHosting = new GrpcBimHostingFactory();
                PluginFactory pluginFactory = new PluginFactory(logger, feaModelApi, workingDirectory, bimHosting.InitGrpcClient(logger));
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
