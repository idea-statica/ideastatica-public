using Autofac;
using Autofac.Extensions.DependencyInjection;
using BimApiFeaLink.Importers;
using FeaApi;
using IdeaStatiCa.BimApiLink;
using IdeaStatiCa.Plugin;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CheckbotClient
{
	public class CheckBotRunner
	{
		public static async Task Run(string checkbotLocation, IFeaApi feaApi, IPluginLogger logger)
		{
			if (logger is null)
			{
				throw new ArgumentNullException(nameof(logger));
			}

			logger.LogInformation($"Starting plugin with checkbot location {checkbotLocation}");
			var workingDirectory = feaApi.GetProjectDir();
			if (!Directory.Exists(workingDirectory))
			{
				logger.LogInformation($"Creating a new project dir '{workingDirectory}'");
				Directory.CreateDirectory(workingDirectory);
			}
			else
			{
				logger.LogInformation($"Using an existing project dir '{workingDirectory}'");
			}

			try
			{
				GrpcBimHostingFactory bimHostingFactory = new GrpcBimHostingFactory();

				var container = BuildContainer(bimHostingFactory.InitGrpcClient(logger), feaApi);

				Model model = container.Resolve<Model>();

				await FeaBimLink.Create("My application name", workingDirectory)
					.WithIdeaStatiCa(checkbotLocation)
					.WithImporters(x => x.RegisterContainer(new AutofacServiceProvider(container)))
					.WithLogger(logger)
					.WithBimHostingFactory(bimHostingFactory)
					.Run(model);
			}
			catch (Exception ex)
			{
				logger.LogError("BimApi failed", ex);
				throw;
			}
		}

		private static IContainer BuildContainer(IProgressMessaging messagingService, IFeaApi feaApi)
		{
			ContainerBuilder builder = new ContainerBuilder();

			// Register FEA application API (geometry, loads, results, ...)
			builder.Register(x => feaApi.Geometry);

			// Register messaging service (progress, ...)
			builder.RegisterInstance(messagingService);

			// Register importers
			builder.RegisterType<CrossSectionImporter>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<MaterialImporter>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<MemberImporter>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<NodeImporter>().AsImplementedInterfaces().SingleInstance();

			builder.RegisterType<Model>().SingleInstance();
			return builder.Build();
		}
	}
}