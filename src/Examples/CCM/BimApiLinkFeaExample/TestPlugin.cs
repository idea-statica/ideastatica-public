﻿using Autofac;
using Autofac.Extensions.DependencyInjection;
using BimApiLinkFeaExample.FeaExampleApi;
using BimApiLinkFeaExample.Importers;
using IdeaStatica.BimApiLink;
using IdeaStatiCa.Plugin;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BimApiLinkFeaExample
{
	public static class TestPlugin
	{
		public static async Task Run(string checkbotLocation, IPluginLogger logger)
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

			IFeaApi feaApi = new FeaApi();

			try
			{
				logger.LogInformation($"Project working directory is {workingDirectory}");
				var link = BimLink.Create("My application name", workingDirectory);

				var container = BuildContainer(link.InitHostingClient(logger), feaApi);

				Model model = container.Resolve<Model>();

				await link
					.WithIdeaStatiCa(checkbotLocation)
					.WithImporters(x => x.RegisterContainer(new AutofacServiceProvider(container)))
					.WithLogger(logger)
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