using IdeaStatiCa.BimApiLink;
using IdeaStatiCa.BimApiLink.Hooks;
using IdeaStatiCa.BimApiLink.Importers;
using IdeaStatiCa.BimApiLink.Persistence;
using IdeaStatiCa.BimApiLink.Plugin;
using IdeaStatiCa.BimImporter;
using IdeaStatiCa.BimImporter.Persistence;
using IdeaStatiCa.BimImporter.Results;
using IdeaStatiCa.Plugin;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yjk
{
	public class YjkBimLink : FeaBimLink
	{
		public YjkBimLink(string applicationName, string projectPath) : base(applicationName, projectPath)
		{
		}

		public static BimLink Create(string applicationName, string checkbotProjectPath)
	=> new YjkBimLink(applicationName, checkbotProjectPath);

		protected override IApplicationBIM CreateAppBim(IPluginLogger logger,
			IBimApiImporter bimApiImporter,
			IPluginHook pluginHook,
			IScopeHook scopeHook,
			IBimUserDataSource userDataSource,
			TaskScheduler taskScheduler,
			IProjectStorage projectStorage,
			IProject projectAdapter,
			IBimImporter bimImporter,
			IFeaModel model,
			bool highlightSelection)
		{
			return new YjkApplication(
				ApplicationName,
				logger,
				projectAdapter,
				projectStorage,
				bimImporter,
				bimApiImporter,
				pluginHook,
				scopeHook,
				userDataSource,
				taskScheduler,
				(Model)model,
				highlightSelection);
		}
	}
}
