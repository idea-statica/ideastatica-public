using Autofac;
using Autofac.Extensions.DependencyInjection;
using IdeaStatiCa.BimApiLink;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.TeklaStructuresPlugin.Hooks;
using IdeaStatiCa.TeklaStructuresPlugin.Importers;
using IdeaStatiCa.TeklaStructuresPlugin.UserData;
using IdeaStatiCa.TeklaStructuresPlugin.Utilities;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace IdeaStatiCa.TeklaStructuresPlugin
{
	public class IdeaStatiCaClient
	{
		private const string LinkName = "Tekla Structures";
		private readonly IPluginLogger pluginLogger;

		public IdeaStatiCaClient(IPluginLogger pluginLogger)
		{
			this.pluginLogger = pluginLogger;
		}

		private IContainer BuildContainer()
		{
			ContainerBuilder builder = new ContainerBuilder();

			builder.RegisterType<CrossSectionImporter>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<MaterialImporter>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<NodeImporter>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<MemberImporter>().SingleInstance().AsImplementedInterfaces();
			builder.RegisterType<ConnectedMemberImporter>().SingleInstance().AsImplementedInterfaces();
			builder.RegisterType<ConnectionImporter>().SingleInstance().AsImplementedInterfaces();
			builder.RegisterType<PlateImporter>().SingleInstance().AsImplementedInterfaces();

			builder.RegisterType<FoldedPlateImporter>().SingleInstance().AsImplementedInterfaces();
			builder.RegisterType<NegativePlateImporter>().SingleInstance().AsImplementedInterfaces();
			builder.RegisterType<WeldImporter>().SingleInstance().AsImplementedInterfaces();
			builder.RegisterType<BoltGridImporter>().SingleInstance().AsImplementedInterfaces();

			builder.RegisterType<WorkPlaneImporter>().SingleInstance().AsImplementedInterfaces();
			builder.RegisterType<CutImporter>().SingleInstance().AsImplementedInterfaces();

			builder.RegisterType<Model>();
			builder.RegisterType<ModelClient>().WithParameter(new TypedParameter(typeof(Tekla.Structures.Model.Model), new Tekla.Structures.Model.Model())).AsImplementedInterfaces().SingleInstance();

			builder.RegisterInstance(pluginLogger).As<IPluginLogger>().SingleInstance();

			builder.RegisterType<AppVisibility>().SingleInstance();

			builder.RegisterType<UserDataSource>().SingleInstance();

			return builder.Build();
		}

		public Task Run()
		{
			IContainer container = BuildContainer();

			string projectPath = CreateProjectDirectory(container.Resolve<IModelClient>());

			BimLink bimLink = TeklaCadBimLink.Create(LinkName, projectPath)
				.WithIdeaStatiCa(GetCheckbotLocation());

			AppVisibility appVisibility = container.Resolve<AppVisibility>();

			Model model = container.Resolve<Model>();
			return bimLink
				.WithLogger(container.Resolve<IPluginLogger>())
				.WithImporters(x => x.RegisterContainer(new AutofacServiceProvider(container)))
				.WithPluginHook(appVisibility)
				.WithItemsComparer(new IdentifierComparer())
				.WithUserDataSource(container.Resolve<UserDataSource>())
				.Run(model);
		}

		public IApplicationBIM GetBimLink(IProgressMessaging progressMessaging = null)
		{
			IContainer container = BuildContainer();

			string projectPath = CreateProjectDirectory(container.Resolve<IModelClient>());

			BimLink bimLink = TeklaCadBimLink.Create(LinkName, projectPath)
				.WithTaskScheduler(TaskScheduler.FromCurrentSynchronizationContext());

			AppVisibility appVisibility = container.Resolve<AppVisibility>();

			Model model = container.Resolve<Model>();
			bimLink
				.WithLogger(container.Resolve<IPluginLogger>())
				.WithImporters(x => x.RegisterContainer(new AutofacServiceProvider(container)))
				.WithPluginHook(appVisibility)
				.WithProgressMessaging(progressMessaging)
				.WithUserDataSource(container.Resolve<UserDataSource>());

			return bimLink.Create(model);
		}

		public string GetProjectPath()
		{
			IContainer container = BuildContainer();
			var model = container.Resolve<IModelClient>();
			return model.GetProjectPath();
		}

		private static string CreateProjectDirectory(IModelClient model)
		{
			string asProjectPath = model.GetProjectPath();

			string projectDirectoryPath = Path.Combine(Path.GetDirectoryName(asProjectPath), Path.GetFileNameWithoutExtension(asProjectPath));
			if (!Directory.Exists(projectDirectoryPath))
			{
				Directory.CreateDirectory(projectDirectoryPath);
			}
			return projectDirectoryPath;
		}

		private static string GetCheckbotLocation()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			string checkbotRoot = Path.GetDirectoryName(assembly.Location)
				?? throw new Exception("Unable to get checkbot app root folder.");

			//checkbot app is in same folder as teklaPlugin
			var checkbotLocation = Path.Combine(checkbotRoot, IdeaStatiCa.Plugin.Constants.CheckbotAppName);
			if (!File.Exists(checkbotLocation))
			{
				//checkbot app is in base folder and teklaPlugin is in net48 of setup
				checkbotLocation = Path.Combine(checkbotRoot, "..\\", IdeaStatiCa.Plugin.Constants.CheckbotAppName);

				if (!File.Exists(checkbotLocation))
				{
					//checkbot app is in net48 folder and teklaPlugin is in base of setup
					checkbotLocation = Path.Combine(checkbotRoot, "net48", IdeaStatiCa.Plugin.Constants.CheckbotAppName);
				}

				if (!File.Exists(checkbotLocation))
				{
					//checkbot app is in net6.0-windows folder and teklaPlugin is in base of setup
					checkbotLocation = Path.Combine(checkbotRoot, "net6.0-windows", IdeaStatiCa.Plugin.Constants.CheckbotAppName);
				}

				if (!File.Exists(checkbotLocation))
				{
					//checkbot app is in net6.0-windows folder and teklaPlugin is in net48  of setup
					checkbotLocation = Path.Combine(checkbotRoot, "..\\net6.0-windows", IdeaStatiCa.Plugin.Constants.CheckbotAppName);
				}
				if (!File.Exists(checkbotLocation))
				{
					//checkbot app is in net48 folder and teklaPlugin is in net6.0-windows  of setup
					checkbotLocation = Path.Combine(checkbotRoot, "..\\net48", IdeaStatiCa.Plugin.Constants.CheckbotAppName);
				}

				if (!File.Exists(checkbotLocation))
				{
					throw new FileNotFoundException($"Checkbot location was not found from this folder: {checkbotRoot} ");
				}
			}

			return checkbotLocation;
		}
	}
}