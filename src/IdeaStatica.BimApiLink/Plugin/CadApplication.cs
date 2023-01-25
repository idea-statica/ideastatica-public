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
			IBimUserDataSource userDataSource,
			TaskScheduler taskScheduler)
			: base(applicationName, project, projectStorage, bimApiImporter, pluginHook, userDataSource, taskScheduler)
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
			return _bimImporter.ImportSelected(items, countryCode);
		}

		protected override void ActivateMethod(List<BIMItemId> items)
		{
			using (CreateScope(CountryCode.None))
			{
				IEnumerable<Identifier<IIdeaObject>> tokens = items
					.Where(x => x.Type != BIMItemType.BIMItemsGroup)
					.Select(x => _project.GetPersistenceToken(x.Id))
					.OfType<Identifier<IIdeaObject>>();

				Select(tokens);
			}
		}

		protected virtual void Select(IEnumerable<Identifier<IIdeaObject>> objects)
		{
			throw new NotImplementedException();
		}

		protected override void Select(IEnumerable<Identifier<IIdeaNode>> nodes, IEnumerable<Identifier<IIdeaMember1D>> members)
		{
			throw new NotImplementedException();
		}
	}
}