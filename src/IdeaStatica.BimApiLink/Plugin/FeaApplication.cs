using IdeaRS.OpenModel;
using IdeaStatica.BimApiLink.Hooks;
using IdeaStatica.BimApiLink.Identifiers;
using IdeaStatica.BimApiLink.Importers;
using IdeaStatica.BimApiLink.Persistence;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter;
using IdeaStatiCa.Plugin;
using System;
using System.Collections.Generic;

namespace IdeaStatica.BimApiLink.Plugin
{
	public class FeaApplication : BimApiApplication
	{
		private readonly IBimImporter _bimImporter;

		public FeaApplication(
			string applicationName,
			IProject project,
			IProjectStorage projectStorage,
			IBimImporter bimImporter,
			IBimApiImporter bimApiImporter,
			IPluginHook pluginHook,
			IScopeHook scopeHook,
			IBimUserDataSource userDataSource)
			: base(applicationName, project, projectStorage, bimApiImporter, pluginHook, scopeHook, userDataSource)
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