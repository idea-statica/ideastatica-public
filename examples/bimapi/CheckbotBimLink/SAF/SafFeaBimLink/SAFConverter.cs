using IdeaRS.OpenModel;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter;
using IdeaStatiCa.BimImporter.BimItems;
using IdeaStatiCa.BimImporter.Results;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.SAF2IOM;
using IdeaStatiCa.SAF2IOM.BimApi;

namespace SafFeaBimLink
{
	internal partial class SAFConverter : ISAFConverter
	{
		private readonly IPluginLogger _pluginLogger;
		private readonly IProject _project;
		private readonly IBimObjectImporter _bimObjectImporter;
		public readonly IProgressMessaging RemoteApp;

		public SAFConverter(IPluginLogger pluginLogger, IProject project, IProgressMessaging remoteApp)
		{
			_pluginLogger = pluginLogger;
			_project = project;
			_bimObjectImporter = BimObjectImporter.Create(_pluginLogger,
				new SAFBimImporterConfiguration(),
				new DefaultResultsProvider(),
				remoteApp);
			RemoteApp = remoteApp;
		}

		public ModelBIM ImportConnections(SAFModel model, CountryCode countryCode)
		{
			IBimImporter importer = BimImporter.Create(model, _project, _pluginLogger);
			return importer.ImportConnections(countryCode);
		}

		public ModelBIM ImportMember(SAFModel model, CountryCode countryCode)
		{
			IBimImporter importer = BimImporter.Create(model, _project, _pluginLogger);
			return importer.ImportMembers(countryCode);
		}

		public ModelBIM Import(SAFModel model, List<BIMItemId> items, CountryCode countryCode)
		{
			IEnumerable<IBimItem> bimItems = items.Select(x => CreateSyncItem(model, x));

			return _bimObjectImporter.Import(
				Enumerable.Empty<IIdeaObject>(),
				bimItems,
				_project,
				countryCode);
		}

		private SyncItem CreateSyncItem(SAFModel model, BIMItemId item)
		{
			PersistenceToken token = (PersistenceToken)_project.GetPersistenceToken(item.Id);
			return new SyncItem(item.Type, model.GetObject(token.SafId));
		}
	}
}
