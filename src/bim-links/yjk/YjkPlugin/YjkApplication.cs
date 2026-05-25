using IdeaRS.OpenModel;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink;
using IdeaStatiCa.BimApiLink.BimApi;
using IdeaStatiCa.BimApiLink.Hooks;
using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.BimApiLink.Importers;
using IdeaStatiCa.BimApiLink.Persistence;
using IdeaStatiCa.BimApiLink.Plugin;
using IdeaStatiCa.BimImporter;
using IdeaStatiCa.Plugin;
using System;
using yjk.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yjk
{
	public class YjkApplication : BimApiApplication
	{
		private readonly IBimImporter _bimImporter;
		private readonly IProject _project;
		private readonly Model _model;
		private readonly IPluginLogger _logger = AppLogger.Instance;

		public YjkApplication(
			string applicationName,
			IPluginLogger logger,
			IProject project,
			IProjectStorage projectStorage,
			IBimImporter bimImporter,
			IBimApiImporter bimApiImporter,
			IPluginHook pluginHook,
			IScopeHook scopeHook,
			IBimUserDataSource userDataSource,
			TaskScheduler taskScheduler,
			Model model,
			bool highlightSelection = true)
			: base(applicationName, logger, project, projectStorage, bimApiImporter, pluginHook, scopeHook, userDataSource, taskScheduler, highlightSelection)
		{
			_bimImporter = bimImporter;
			_project = project;
			_model = model;
		}

		protected override ModelBIM ImportSelection(CountryCode countryCode, RequestedItemsType requestedType)
		{
			_logger.LogInformation($"YjkApplication.ImportSelection: countryCode={countryCode}, requestedType={requestedType}");
			switch (requestedType)
			{
				case RequestedItemsType.Connections:
					return _bimImporter.ImportConnections(countryCode);

				case RequestedItemsType.Substructure:
					return _bimImporter.ImportMembers(countryCode);

				case RequestedItemsType.Members2D:
					return _bimImporter.ImportMembers2D(countryCode);

				default:
					throw new NotImplementedException();
			}
		}

		protected override List<ModelBIM> Synchronize(CountryCode countryCode, List<BIMItemsGroup> items)
		{
			_logger.LogInformation("YjkApplication.Synchronize");
			_model.Refresh();
			return _bimImporter.ImportSelected(items, countryCode);
		}

		protected override void Select(IEnumerable<Identifier<IIdeaNode>> nodes, IEnumerable<Identifier<IIdeaMember1D>> members)
		{
			// no nothing
		}
	}
}
