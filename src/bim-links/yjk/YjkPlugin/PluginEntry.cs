using Autofac;
using Autofac.Extensions.DependencyInjection;
using IdeaStatiCa.BimApiLink;
using IdeaStatiCa.BimImporter;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.PluginLogger;
using System;
using System.IO;
using System.Windows.Threading;
using yjk.FeaApis;
using yjk.Importers;
using yjk.ViewModels;

namespace yjk
{
	public static class PluginEntry
	{
		public static Dispatcher YjkDispatcher { get; set; }

		public static void Run(string workingDirectory)
		{
#if DEBUG
			System.Diagnostics.Debugger.Launch();
#endif

			SerilogFacade.Initialize("IdeaYJKPlugin.log");

			IPluginLogger logger = AppLogger.Instance;
			logger.LogInformation($"PluginEntry.Run workDir={workingDirectory}");

			string checkbotLocation = Properties.Settings.Default.CheckbotLocation;
			logger.LogInformation($"Starting plugin with checkbot location {checkbotLocation}");

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

				YjkBimLink.Create("YJK", workingDirectory)
					.WithIdeaStatiCa(checkbotLocation)
					.WithImporters(x => x.RegisterContainer(new AutofacServiceProvider(container)))
					.WithResultsImporters(x => x.RegisterImporter(container.Resolve<ResultsImporter>()))
					.WithLogger(logger)
					.WithBimHostingFactory(bimHostingFactory)
					.WithBimImporterConfiguration(bimImporterConfiguration)
					.Run(model)
					.GetAwaiter().GetResult();
			}
			catch (Exception ex)
			{
				logger.LogError("PluginEntry failed", ex);
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
			builder.RegisterType<NodeImporter>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<MemberImporter>().AsImplementedInterfaces().SingleInstance();

			builder.RegisterType<LoadCaseImporter>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<LoadGroupImporter>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<LoadCombinationImporter>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<ResultsImporter>().SingleInstance();

			builder.RegisterType<Model>().SingleInstance();
			return builder.Build();
		}
	}
}
