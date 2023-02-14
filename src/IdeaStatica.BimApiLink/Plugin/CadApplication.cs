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
using System.Linq;
using System.Threading.Tasks;

namespace IdeaStatica.BimApiLink.Plugin
{
	public class CadApplication : BimApiApplication
	{
		private readonly IBimImporter _bimImporter;
		private readonly IProject _project;


		public CadApplication(
			string applicationName,
			IProject project,
			IProjectStorage projectStorage,
			IBimImporter bimImporter,
			IBimApiImporter bimApiImporter,
			IPluginHook pluginHook,
			IScopeHook scopeHook,
			IBimUserDataSource userDataSource,
			TaskScheduler taskScheduler)
			: base(applicationName, project, projectStorage, bimApiImporter, pluginHook, scopeHook, userDataSource, taskScheduler)
		{
			_bimImporter = bimImporter;
			_project = project;
		}

		public override bool IsCAD() => true;

		protected override ModelBIM ImportSelection(CountryCode countryCode, RequestedItemsType requestedType)
		{
			switch (requestedType)
			{
				case RequestedItemsType.Connections:
				case RequestedItemsType.Substructure:
					return _bimImporter.ImportConnections(countryCode);

				case RequestedItemsType.SingleConnection:
					return _bimImporter.ImportSingleConnection(countryCode);

				default:
					throw new NotImplementedException();
			}
		}

		protected override List<ModelBIM> Synchronize(CountryCode countryCode, List<BIMItemsGroup> items)
		{
			items.ForEach(i => i.Items.Add(new BIMItemId() { Id = i.Id, Type = BIMItemType.BIMItemsGroup }));
			return _bimImporter.ImportSelected(items, countryCode);
		}

		protected override void ActivateMethod(List<BIMItemId> items)
		{
			using (CreateScope(CountryCode.None))
			{
				IEnumerable<IIdeaObject> tokens = items
					.Where(x => x.Type != BIMItemType.BIMItemsGroup)
					.Select(x => _project.GetBimObject(x.Id));


				Select(tokens);
			}
		}

		protected virtual void Select(IEnumerable<IIdeaObject> objects)
		{
			throw new NotImplementedException();
		}

		protected override void Select(IEnumerable<Identifier<IIdeaNode>> nodes, IEnumerable<Identifier<IIdeaMember1D>> members)
		{
			throw new NotImplementedException();
		}
	}
}