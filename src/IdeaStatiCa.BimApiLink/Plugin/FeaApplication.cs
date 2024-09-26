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
		public readonly IBimImporter BimImporter;

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
			BimImporter = bimImporter;
		}

		protected override ModelBIM ImportSelection(CountryCode countryCode, RequestedItemsType requestedType)
		{
			switch (requestedType)
			{
				case RequestedItemsType.Connections:
					return BimImporter.ImportConnections(countryCode);

				case RequestedItemsType.Substructure:
					return BimImporter.ImportMembers(countryCode);

				default:
					throw new NotImplementedException();
			}
		}

		protected override List<ModelBIM> Synchronize(CountryCode countryCode, List<BIMItemsGroup> items)
		{
			return BimImporter.ImportSelected(items, countryCode);
		}

		protected override void Select(IEnumerable<Identifier<IIdeaNode>> nodes, IEnumerable<Identifier<IIdeaMember1D>> members)
		{
			// no nothing
		}
	}
}
