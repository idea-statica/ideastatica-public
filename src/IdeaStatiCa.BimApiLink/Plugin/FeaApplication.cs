using IdeaRS.OpenModel;
using IdeaStatiCa.BimApiLink.Hooks;
using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.BimApiLink.Importers;
using IdeaStatiCa.BimApiLink.Persistence;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter;
using IdeaStatiCa.Plugin;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdeaStatiCa.BimApiLink.Plugin
{
	public class FeaApplication : BimApiApplication
	{
		private readonly IBimImporter _bimImporter;

		public FeaApplication(
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
			bool highlightSelection = true)
			: base(applicationName, logger, project, projectStorage, bimApiImporter, pluginHook, scopeHook, userDataSource, taskScheduler, highlightSelection)
		{
			_bimImporter = bimImporter;
		}

		protected override ModelBIM ImportSelection(CountryCode countryCode, RequestedItemsType requestedType)
		{
			switch (requestedType)
			{
				case RequestedItemsType.Connections:
					return _bimImporter.ImportConnections(countryCode);

				case RequestedItemsType.Substructure:
					return _bimImporter.ImportMembers(countryCode);

				default:
					throw new NotImplementedException();
			}
		}

		protected override List<ModelBIM> Synchronize(CountryCode countryCode, List<BIMItemsGroup> items)
		{
			return _bimImporter.ImportSelected(items, countryCode);
		}

		protected override void Select(IEnumerable<Identifier<IIdeaNode>> nodes, IEnumerable<Identifier<IIdeaMember1D>> members)
		{
			// no nothing
		}
	}
}