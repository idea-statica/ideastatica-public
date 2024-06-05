using Autofac;
using Autofac.Extensions.DependencyInjection;
using BimApiLinkCadExample.BimApi;
using BimApiLinkCadExample.CadExampleApi;
using BimApiLinkCadExample.Hooks;
using BimApiLinkCadExample.Importers;
using IdeaStatiCa.BimApiLink;
using IdeaStatiCa.Plugin;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BimApiLinkCadExample
{
	public static class TestPlugin
	{
		public static async Task Run(string checkbotLocation, ICadApi cadApi, IPluginLogger logger)
		{
			if (logger is null)
			{
				throw new ArgumentNullException(nameof(logger));
			}

			logger.LogInformation($"Starting plugin with checkbot location {checkbotLocation}");
			var workingDirectory = Path.GetFullPath("BimApiExampleProj");
			if (!Directory.Exists(workingDirectory))
			{
				Directory.CreateDirectory(workingDirectory);
			}

			//IFeaApi feaApi = new FeaApi();

			try
			{
				GrpcBimHostingFactory bimHostingFactory = new GrpcBimHostingFactory();

				logger.LogInformation($"Project working directory is {workingDirectory}");

				var container = BuildContainer(bimHostingFactory.InitGrpcClient(logger), cadApi);

				Model model = container.Resolve<Model>();

				await CadBimLink.Create("My application name", workingDirectory)
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

		private static IContainer BuildContainer(IProgressMessaging messagingService, ICadApi cadApiModel)
		{
			ContainerBuilder builder = new ContainerBuilder();

			// Register CAD application API
			builder.Register(x => cadApiModel.Geometry);

			// Register messaging service (progress, ...)
			builder.RegisterInstance(messagingService);

			// Register importers
			// Commented importers represent future possible.
			builder.RegisterType<CrossSectionImporter>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<MaterialImporter>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<NodeImporter>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<MemberImporter>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<ConnectedMemberImporter>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<ConnectionImporter>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<PlateImporter>().SingleInstance().AsImplementedInterfaces();
			//builder.RegisterType<FoldedPlateImporter>().SingleInstance().AsImplementedInterfaces();
			builder.RegisterType<NegativePlateImporter>().SingleInstance().AsImplementedInterfaces();
			//builder.RegisterType<WeldImporter>().SingleInstance().AsImplementedInterfaces();
			builder.RegisterType<BoltGridImporter>().SingleInstance().AsImplementedInterfaces();
			//builder.RegisterType<AnchorGridImporter>().SingleInstance().AsImplementedInterfaces();
			//builder.RegisterType<WorkPlaneImporter>().SingleInstance().AsImplementedInterfaces();
			builder.RegisterType<CutImporter>().SingleInstance().AsImplementedInterfaces();

			builder.RegisterType<Model>().SingleInstance();

			//Plugin Hook can be introduced for selection and transactions.
			//builder.RegisterType<PluginHook>().SingleInstance();

			return builder.Build();
		}
	}
}