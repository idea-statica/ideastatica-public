using IdeaRS.OpenModel;
using IdeaStatiCa.BimImporter;
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
			IBimImporter importer = BimImporter.Create(model, _project, _pluginLogger, configuration: new SAFBimImporterConfiguration());
			return importer.ImportConnections(countryCode);
		}

		public ModelBIM ImportMember(SAFModel model, CountryCode countryCode)
		{
			IBimImporter importer = BimImporter.Create(model, _project, _pluginLogger, configuration: new SAFBimImporterConfiguration());
			return importer.ImportMembers(countryCode);
		}

		public IReadOnlyList<ModelBIM> Import(SAFModel model, IEnumerable<BIMItemsGroup> groups, CountryCode countryCode)
		{
			IBimImporter importer = BimImporter.Create(model, _project, _pluginLogger, configuration: new SAFBimImporterConfiguration());

			var grouped = groups.GroupBy(x => x.Type);
			var retVal = new List<ModelBIM>();

			foreach (var group in grouped)
			{
				ModelBIM modelBim = group.Key switch
				{
					RequestedItemsType.Connections => importer.ImportConnections(countryCode),
					RequestedItemsType.Substructure => importer.ImportMembers(countryCode),
					RequestedItemsType.SingleConnection => importer.ImportSingleConnection(countryCode),
					RequestedItemsType.WholeModel => importer.ImportWholeModel(countryCode),
					RequestedItemsType.Members2D => importer.ImportMembers2D(countryCode),
					_ => throw new NotSupportedException($"{group.Key} is not supported"),
				};
				retVal.Add(modelBim);
			}

			return retVal;			
		}		
	}
}
