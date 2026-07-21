using Autofac;
using Autofac.Extensions.DependencyInjection;
using IdeaStatiCa.BIM.Common;
using IdeaStatiCa.BimApiLink;
using IdeaStatiCa.BimApiLink.Persistence;
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
        private readonly SorterSettings sorterSettings;

        public IdeaStatiCaClient(IPluginLogger pluginLogger, SorterSettings sorterSettings = null)
        {
            this.pluginLogger = pluginLogger;
            this.sorterSettings = sorterSettings;
            pluginLogger?.LogInformation("IdeaStatiCaClient constructor called");
            pluginLogger?.LogInformation($"Plugin logger type: {pluginLogger?.GetType().FullName}");
        }

        private IContainer BuildContainer()
        {
            pluginLogger?.LogInformation("BuildContainer - Start");
            try
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
                builder.RegisterType<AnchorGridImporter>().SingleInstance().AsImplementedInterfaces();
                builder.RegisterType<ConcreteBlockImporter>().SingleInstance().AsImplementedInterfaces();

                builder.RegisterType<WorkPlaneImporter>().SingleInstance().AsImplementedInterfaces();
                builder.RegisterType<CutImporter>().SingleInstance().AsImplementedInterfaces();

                var resolvedSorterSettings = sorterSettings ?? new SorterSettings
                {
                    EnlargeNodeXin = 1.6,
                    EnlargeNodeXout = 1.6,
                    EnlargeNodeY = 1.7,
                    EnlargeNodeZ = 1.7,
                };
                builder.RegisterInstance(resolvedSorterSettings).AsSelf().SingleInstance();

                builder.RegisterType<Model>();
                builder.RegisterType<ModelClient>().WithParameter(new TypedParameter(typeof(Tekla.Structures.Model.Model), new Tekla.Structures.Model.Model())).AsImplementedInterfaces().SingleInstance();

                builder.RegisterInstance(pluginLogger).As<IPluginLogger>().SingleInstance();

                builder.RegisterType<AppVisibility>().SingleInstance();

                builder.RegisterType<UserDataSource>().SingleInstance();

                pluginLogger?.LogInformation("BuildContainer - Building container");
                var container = builder.Build();
                pluginLogger?.LogInformation("BuildContainer - Container built successfully");
                return container;
            }
            catch (Exception ex)
            {
                pluginLogger?.LogError($"BuildContainer - Error: {ex.Message}", ex);
                throw;
            }
        }

        public Task Run()
        {
            pluginLogger?.LogInformation("Run - Method started");
            try
            {
                IContainer container = BuildContainer();

                pluginLogger?.LogInformation("Run - Resolving IModelClient");
                var modelClient = container.Resolve<IModelClient>();
                pluginLogger?.LogInformation($"Run - IModelClient resolved: {modelClient?.GetType().FullName}");

                pluginLogger?.LogInformation("Run - Creating project directory");
                string projectPath = CreateProjectDirectory(modelClient);
                pluginLogger?.LogInformation($"Run - Project path: {projectPath}");

                pluginLogger?.LogInformation("Run - Getting Checkbot location");
                string checkbotLocation = GetCheckbotLocation();
                pluginLogger?.LogInformation($"Run - Checkbot location: {checkbotLocation}");

                pluginLogger?.LogInformation("Run - Creating BimLink");
                BimLink bimLink = TeklaCadBimLink.Create(LinkName, projectPath)
                    .WithIdeaStatiCa(checkbotLocation);

                pluginLogger?.LogInformation("Run - Resolving AppVisibility");
                AppVisibility appVisibility = container.Resolve<AppVisibility>();

                pluginLogger?.LogInformation("Run - Resolving Model");
                Model model = container.Resolve<Model>();

                pluginLogger?.LogInformation("Run - Configuring BimLink with logger, importers, hooks");
                var task = bimLink
                    .WithLogger(container.Resolve<IPluginLogger>())
                    .WithImporters(x => x.RegisterContainer(new AutofacServiceProvider(container)))
                    .WithPluginHook(appVisibility)
                    .WithItemsComparer(new IdentifierComparer())
                    .WithUserDataSource(container.Resolve<UserDataSource>())
                    .Run(model);

                pluginLogger?.LogInformation("Run - BimLink.Run started");
                return task;
            }
            catch (Exception ex)
            {
                pluginLogger?.LogError($"Run - Error: {ex.Message}", ex);
                throw;
            }
        }

        public IApplicationBIM GetBimLink(IProgressMessaging progressMessaging = null, IProjectStorage projectStorage = null)
        {
            pluginLogger?.LogInformation("GetBimLink - Method started");
            pluginLogger?.LogInformation($"GetBimLink - ProgressMessaging: {(progressMessaging != null ? "provided" : "null")}");
            try
            {
                IContainer container = BuildContainer();

                pluginLogger?.LogInformation("GetBimLink - Resolving IModelClient");
                var modelClient = container.Resolve<IModelClient>();

                pluginLogger?.LogInformation("GetBimLink - Creating project directory");
                string projectPath = CreateProjectDirectory(modelClient);
                pluginLogger?.LogInformation($"GetBimLink - Project path: {projectPath}");

                pluginLogger?.LogInformation("GetBimLink - Getting Checkbot location");
                string checkbotLocation = GetCheckbotLocation();
                pluginLogger?.LogInformation($"GetBimLink - Checkbot location: {checkbotLocation}");

                pluginLogger?.LogInformation("GetBimLink - Creating BimLink with TaskScheduler");
                BimLink bimLink = TeklaCadBimLink.Create(LinkName, projectPath)
                    .WithTaskScheduler(TaskScheduler.FromCurrentSynchronizationContext())
                    .WithIdeaStatiCa(checkbotLocation);

                pluginLogger?.LogInformation("GetBimLink - Resolving AppVisibility");
                AppVisibility appVisibility = container.Resolve<AppVisibility>();

                pluginLogger?.LogInformation("GetBimLink - Resolving Model");
                Model model = container.Resolve<Model>();

                pluginLogger?.LogInformation("GetBimLink - Configuring BimLink");
                bimLink
                    .WithLogger(container.Resolve<IPluginLogger>())
                    .WithImporters(x => x.RegisterContainer(new AutofacServiceProvider(container)))
                    .WithPluginHook(appVisibility)
                    .WithProgressMessaging(progressMessaging)
                    .WithProjectStorage(projectStorage)
                    .WithItemsComparer(new IdentifierComparer())
                    .WithUserDataSource(container.Resolve<UserDataSource>());

                pluginLogger?.LogInformation("GetBimLink - Creating IApplicationBIM");
                var result = bimLink.Create(model);
                pluginLogger?.LogInformation($"GetBimLink - IApplicationBIM created: {result?.GetType().FullName}");
                return result;
            }
            catch (Exception ex)
            {
                pluginLogger?.LogError($"GetBimLink - Error: {ex.Message}", ex);
                throw;
            }
        }

        public string GetProjectPath()
        {
            pluginLogger?.LogInformation("GetProjectPath - Method started");
            try
            {
                IContainer container = BuildContainer();
                pluginLogger?.LogInformation("GetProjectPath - Resolving IModelClient");
                var model = container.Resolve<IModelClient>();
                string path = model.GetProjectPath();
                pluginLogger?.LogInformation($"GetProjectPath - Path: {path}");
                return path;
            }
            catch (Exception ex)
            {
                pluginLogger?.LogError($"GetProjectPath - Error: {ex.Message}", ex);
                throw;
            }
        }

        private string CreateProjectDirectory(IModelClient model)
        {
            pluginLogger?.LogInformation("CreateProjectDirectory - Method started");
            try
            {
                string asProjectPath = model.GetProjectPath();
                pluginLogger?.LogInformation($"CreateProjectDirectory - Model project path: {asProjectPath}");

                string projectDirectoryPath = Path.Combine(Path.GetDirectoryName(asProjectPath), Path.GetFileNameWithoutExtension(asProjectPath));
                pluginLogger?.LogInformation($"CreateProjectDirectory - Target directory: {projectDirectoryPath}");

                if (!Directory.Exists(projectDirectoryPath))
                {
                    pluginLogger?.LogInformation("CreateProjectDirectory - Directory does not exist, creating");
                    Directory.CreateDirectory(projectDirectoryPath);
                    pluginLogger?.LogInformation("CreateProjectDirectory - Directory created");
                }
                else
                {
                    pluginLogger?.LogInformation("CreateProjectDirectory - Directory already exists");
                }

                return projectDirectoryPath;
            }
            catch (Exception ex)
            {
                pluginLogger?.LogError($"CreateProjectDirectory - Error: {ex.Message}", ex);
                throw;
            }
        }

        private string GetCheckbotLocation()
        {
            pluginLogger?.LogInformation("GetCheckbotLocation - Method started");
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                pluginLogger?.LogInformation($"GetCheckbotLocation - Assembly location: {assembly.Location}");

                string checkbotRoot = Path.GetDirectoryName(assembly.Location)
                    ?? throw new Exception("Unable to get checkbot app root folder.");
                pluginLogger?.LogInformation($"GetCheckbotLocation - Checkbot root: {checkbotRoot}");
                pluginLogger?.LogInformation($"GetCheckbotLocation - Looking for: {IdeaStatiCa.Plugin.Constants.CheckbotAppName}");

                //checkbot app is in same folder as teklaPlugin
                var checkbotLocation = Path.Combine(checkbotRoot, IdeaStatiCa.Plugin.Constants.CheckbotAppName);
                pluginLogger?.LogInformation($"GetCheckbotLocation - Trying: {checkbotLocation}");
                if (!File.Exists(checkbotLocation))
                {
                    //checkbot app is in net8.0-windows folder and teklaPlugin is in net48/TeklaPlugin subfolder (DEBUG build)
                    checkbotLocation = Path.Combine(checkbotRoot, "..\\..\\net8.0-windows", IdeaStatiCa.Plugin.Constants.CheckbotAppName);
                    pluginLogger?.LogInformation($"GetCheckbotLocation - Trying: {checkbotLocation}");
                }
                if (!File.Exists(checkbotLocation))
                {
                    //checkbot app is in net8.0-windows folder and teklaPlugin is in net48/TeklaPlugin subfolder (DEBUG build)
                    checkbotLocation = Path.Combine(checkbotRoot, "..\\..\\net10.0-windows", IdeaStatiCa.Plugin.Constants.CheckbotAppName);
                    pluginLogger?.LogInformation($"GetCheckbotLocation - Trying: {checkbotLocation}");
                }


                if (!File.Exists(checkbotLocation))
                {
                    //checkbot app is in base folder and teklaPlugin is in net48 of setup
                    checkbotLocation = Path.Combine(checkbotRoot, "..\\", IdeaStatiCa.Plugin.Constants.CheckbotAppName);
                    pluginLogger?.LogInformation($"GetCheckbotLocation - Trying: {checkbotLocation}");
                }

                if (!File.Exists(checkbotLocation))
                {
                    //checkbot app is in net48 folder and teklaPlugin is in base of setup
                    checkbotLocation = Path.Combine(checkbotRoot, "net48", IdeaStatiCa.Plugin.Constants.CheckbotAppName);
                    pluginLogger?.LogInformation($"GetCheckbotLocation - Trying: {checkbotLocation}");
                }

                if (!File.Exists(checkbotLocation))
                {
                    //checkbot app is in net8.0-windows folder and teklaPlugin is in base of setup
                    checkbotLocation = Path.Combine(checkbotRoot, "net8.0-windows", IdeaStatiCa.Plugin.Constants.CheckbotAppName);
                    pluginLogger?.LogInformation($"GetCheckbotLocation - Trying: {checkbotLocation}");
                }

                if (!File.Exists(checkbotLocation))
                {
                    //checkbot app is in net8.0-windows folder and teklaPlugin is in net48  of setup
                    checkbotLocation = Path.Combine(checkbotRoot, "..\\net8.0-windows", IdeaStatiCa.Plugin.Constants.CheckbotAppName);
                    pluginLogger?.LogInformation($"GetCheckbotLocation - Trying: {checkbotLocation}");
                }
                if (!File.Exists(checkbotLocation))
                {
                    //checkbot app is in net10.0-windows folder and teklaPlugin is in base of setup
                    checkbotLocation = Path.Combine(checkbotRoot, "net10.0-windows", IdeaStatiCa.Plugin.Constants.CheckbotAppName);
                    pluginLogger?.LogInformation($"GetCheckbotLocation - Trying: {checkbotLocation}");
                }

                if (!File.Exists(checkbotLocation))
                {
                    //checkbot app is in net10.0-windows folder and teklaPlugin is in net48  of setup
                    checkbotLocation = Path.Combine(checkbotRoot, "..\\net10.0-windows", IdeaStatiCa.Plugin.Constants.CheckbotAppName);
                    pluginLogger?.LogInformation($"GetCheckbotLocation - Trying: {checkbotLocation}");
                }

                if (!File.Exists(checkbotLocation))
                {
                    //checkbot app is in net48 folder and teklaPlugin is in net8.0-windows  of setup
                    checkbotLocation = Path.Combine(checkbotRoot, "..\\net48", IdeaStatiCa.Plugin.Constants.CheckbotAppName);
                    pluginLogger?.LogInformation($"GetCheckbotLocation - Trying: {checkbotLocation}");
                }

                if (!File.Exists(checkbotLocation))
                {
                    pluginLogger?.LogError($"GetCheckbotLocation - Checkbot not found. Root folder: {checkbotRoot}");
                    throw new FileNotFoundException($"Checkbot location was not found from this folder: {checkbotRoot} ");
                }

                pluginLogger?.LogInformation($"GetCheckbotLocation - Checkbot found at: {checkbotLocation}");
                return checkbotLocation;
            }
            catch (Exception ex)
            {
                pluginLogger?.LogError($"GetCheckbotLocation - Error: {ex.Message}", ex);
                throw;
            }
        }
    }
}
