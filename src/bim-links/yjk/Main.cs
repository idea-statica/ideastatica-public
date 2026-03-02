using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using System.IO;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.BimApiLink;
using yjk.ViewModels;
using yjk.FeaApis;
using yjk.Importers;
using yjk.Helpers;
using System.Diagnostics;

namespace yjk
{
	public class Main
	{
		static WindowHelper _windowHelper;

		[CommandMethod("test_idea_plugin")]
		public void Test()
		{
			MessageBox.Show("Import completed");
		}

		[CommandMethod("idea_statica")]
		public void Run()
		{
			if (_windowHelper == null)
			{
				_windowHelper = new WindowHelper();
				_windowHelper.Show();
			}

			try
			{
				Task.Run(async () =>
				{
					try
					{
						Debug.WriteLine("Before");
						await RunAsync();
						Debug.WriteLine("After");
					}
					catch (Exception ex)
					{
						Debug.WriteLine(ex.ToString());
					}
				});
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				throw; // or swallow depending on host
			}
		}

		public static async Task RunAsync()
		{
			string checkbotLocation = Properties.Settings.Default.CheckbotLocation;

			AppLogger logger = new AppLogger(Dispatcher.CurrentDispatcher) { EnableDebug = true, };
			IFeaApi feaApi = new FeaApi();

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

			try
			{
				GrpcBimHostingFactory bimHostingFactory = new GrpcBimHostingFactory();

				logger.LogInformation($"Project working directory is {workingDirectory}");

				var container = BuildContainer(bimHostingFactory.InitGrpcClient(logger), feaApi);

				Model model = container.Resolve<Model>();

				await FeaBimLink.Create("My application name", workingDirectory)
					.WithIdeaStatiCa(checkbotLocation)
					.WithImporters(x => x.RegisterContainer(new AutofacServiceProvider(container)))
					.WithResultsImporters(x => x.RegisterImporter(container.Resolve<ResultsImporter>()))
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
			builder.Register(x => feaApi.Loads);
			builder.Register(x => feaApi.Results);

			// Register messaging service (progress, ...)
			builder.RegisterInstance(messagingService);

			// Register importers
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
