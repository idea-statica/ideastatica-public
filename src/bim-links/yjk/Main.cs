using Autofac;
using Autofac.Extensions.DependencyInjection;
using IdeaStatiCa.BimApiLink;
using IdeaStatiCa.BimImporter;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.PluginLogger;
using Newtonsoft.Json;
using Register;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using yjk.FeaApis;
using yjk.Helpers;
using yjk.Importers;
using yjk.ViewModels;

namespace yjk
{
	public class Main
	{
		private static WindowHelper _windowHelper;
		private static IPluginLogger _logger = AppLogger.Instance;		

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
						await RunAsync();
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
			//_logger.EnableDebug = true;

			_logger.LogInformation("Main.RunAsync");

			string checkbotLocation = Properties.Settings.Default.CheckbotLocation;

			IFeaApi feaApi = new FeaApi();

			if (_logger is null)
			{
				throw new ArgumentNullException(nameof(_logger));
			}

			_logger.LogInformation($"Starting plugin with checkbot location {checkbotLocation}");

			//var workingDirectory = Path.GetFullPath("BimApiExampleProj");
			var workingDirectory = Directory.GetCurrentDirectory();

			var yjkFiles = Directory.GetFiles(workingDirectory, "*.yjk");
			var yjkFileName = "";
			if (yjkFiles.Length == 1)
			{
				yjkFileName = Path.GetFileNameWithoutExtension(yjkFiles[0]);
			}

			var fullWorkingDirectory = Path.Combine(workingDirectory, "IdeaStatiCa-" + yjkFileName);

			if (!Directory.Exists(fullWorkingDirectory))
			{
				Directory.CreateDirectory(fullWorkingDirectory);
			}

			/*
			//Read json file to prevent duplicate cross section and material id
			var jsonPath = Path.Combine(fullWorkingDirectory, "bimapi-data.json");
			if (File.Exists(jsonPath))
			{
				string jsonString = File.ReadAllText(jsonPath);
				var bimApiData = JsonConvert.DeserializeObject<BimApiData>(jsonString);
			}
			*/

			try
			{
				GrpcBimHostingFactory bimHostingFactory = new GrpcBimHostingFactory();

				_logger.LogInformation($"Project working directory is {fullWorkingDirectory}");

				var container = BuildContainer(bimHostingFactory.InitGrpcClient(_logger), feaApi);

				Model model = container.Resolve<Model>();

				BimImporterConfiguration bimImporterConfiguration = new BimImporterConfiguration
				{ 
					AutoCreateConnFromTwoMembers = false 
				};

				await FeaBimLink.Create("YJK", fullWorkingDirectory)
					.WithIdeaStatiCa(checkbotLocation)
					.WithImporters(x => x.RegisterContainer(new AutofacServiceProvider(container)))
					.WithResultsImporters(x => x.RegisterImporter(container.Resolve<ResultsImporter>()))
					.WithLogger(_logger)
					.WithBimHostingFactory(bimHostingFactory)
					.WithBimImporterConfiguration(bimImporterConfiguration)
					.Run(model);
			}
			catch (Exception ex)
			{
				_logger.LogError("BimApi failed", ex);
				throw;
			}
		}

		private static IContainer BuildContainer(IProgressMessaging messagingService, IFeaApi feaApi)
		{
			ContainerBuilder builder = new ContainerBuilder();

			// Register FEA application API (geometry, loads, results, ...)
			builder.Register(x => feaApi.Geometry);
			builder.Register(x => feaApi.Load);
			builder.Register(x => feaApi.Result);
			builder.Register(x => feaApi.CrossSection);
			builder.Register(x => feaApi.MaterialApi);

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
