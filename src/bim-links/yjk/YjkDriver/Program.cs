using Autofac;
using Autofac.Extensions.DependencyInjection;
using IdeaStatiCa.BimApiLink;
using IdeaStatiCa.BimImporter;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.PluginLogger;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using yjk.FeaApis;
using yjk.Importers;
using yjk.ViewModels;

namespace yjk
{
	internal static class Program
	{
		[STAThread]
		static void Main(string[] args)
		{

	#if DEBUG
      System.Diagnostics.Debugger.Launch();
	#endif


			SerilogFacade.Initialize("IdeaYJKPlugin.log");

			// args[0] = YJK host process PID
			// args[1] = full working directory path
			if (args.Length < 2)
			{
				Console.Error.WriteLine("Usage: YjkDriver.exe <hostPid> <workingDirectory>");
				Environment.Exit(1);
			}

			if (!int.TryParse(args[0], out int hostPid))
			{
				Console.Error.WriteLine($"Invalid host PID: {args[0]}");
				Environment.Exit(1);
			}

			string workingDirectory = args[1];

			RunAsync(hostPid, workingDirectory).GetAwaiter().GetResult();
		}

		private static async Task RunAsync(int hostPid, string workingDirectory)
		{
			IPluginLogger logger = AppLogger.Instance;
			logger.LogInformation($"YjkDriver.RunAsync hostPid={hostPid} workDir={workingDirectory}");

			string checkbotLocation = Properties.Settings.Default.CheckbotLocation;
			logger.LogInformation($"Starting driver with checkbot location {checkbotLocation}");

			if (!Directory.Exists(workingDirectory))
				Directory.CreateDirectory(workingDirectory);

			try
			{
				GrpcBimHostingFactory bimHostingFactory = new GrpcBimHostingFactory();
				IFeaApi feaApi = new FeaApi();

				var container = BuildContainer(bimHostingFactory.InitGrpcClient(logger), feaApi);
				Model model = container.Resolve<Model>();

				BimImporterConfiguration bimImporterConfiguration = new BimImporterConfiguration
				{
					AutoCreateConnFromTwoMembers = false
				};

				await YjkBimLink.Create("YJK", workingDirectory)
					.WithIdeaStatiCa(checkbotLocation)
					.WithImporters(x => x.RegisterContainer(new AutofacServiceProvider(container)))
					.WithResultsImporters(x => x.RegisterImporter(container.Resolve<ResultsImporter>()))
					.WithLogger(logger)
					.WithBimHostingFactory(bimHostingFactory)
					.WithBimImporterConfiguration(bimImporterConfiguration)
					.Run(model);
			}
			catch (Exception ex)
			{
				logger.LogError("YjkDriver failed", ex);
				throw;
			}
		}

		private static IContainer BuildContainer(IProgressMessaging messagingService, IFeaApi feaApi)
		{
			ContainerBuilder builder = new ContainerBuilder();

			builder.Register(x => feaApi.Geometry);
			builder.Register(x => feaApi.Load);
			builder.Register(x => feaApi.Result);
			builder.Register(x => feaApi.CrossSection);
			builder.Register(x => feaApi.MaterialApi);

			builder.RegisterInstance(messagingService);

			builder.RegisterType<CrossSectionImporter>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<MaterialImporter>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<MemberImporter>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<NodeImporter>().AsImplementedInterfaces().SingleInstance();

			builder.RegisterType<LoadCaseImporter>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<LoadGroupImporter>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<LoadCombinationImporter>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<ResultsImporter>().SingleInstance();

			builder.RegisterType<Model>().SingleInstance();
			return builder.Build();
		}
	}
}
