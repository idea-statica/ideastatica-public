using IdeaStatiCa.BimApiLink;
using IdeaStatiCa.BimApiLink.Hooks;
using IdeaStatiCa.BimApiLink.Importers;
using IdeaStatiCa.BimApiLink.Persistence;
using IdeaStatiCa.BimImporter;
using IdeaStatiCa.Plugin;
using System.Threading.Tasks;

namespace IdeaStatiCa.TeklaStructuresPlugin
{
	public class TeklaCadBimLink : CadBimLink
	{
		public TeklaCadBimLink(string applicationName, string projectPath) : base(applicationName, projectPath)
		{
		}
		public static new BimLink Create(string applicationName, string checkbotProjectPath) => new TeklaCadBimLink(applicationName, checkbotProjectPath);

		protected override IApplicationBIM CreateApplicationInstance(
			IPluginLogger pluginLogger,
			IBimApiImporter bimApiImporter,
			IPluginHook pluginHook,
			IScopeHook scopeHook,
			IBimUserDataSource userDataSource,
			IProjectStorage projectStorage,
			IProject projectAdapter,
			IBimImporter bimImporter,
			TaskScheduler taskScheduler)
		{
			return new TeklaStructuresApplication(
							ApplicationName,
							pluginLogger,
							projectAdapter,
							projectStorage,
							bimImporter,
							bimApiImporter,
							pluginHook,
							scopeHook,
							userDataSource,
							taskScheduler);
		}
	}
}
