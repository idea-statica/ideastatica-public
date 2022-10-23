using IdeaRS.OpenModel;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.BimItems;
using IdeaStatiCa.BimImporter.Importers;
using IdeaStatiCa.BimImporter.Results;
using IdeaStatiCa.Plugin;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.BimImporter
{
	/// <summary>
	/// Class BimObjectImporter.
	/// Implements the <see cref="IdeaStatiCa.BimImporter.IBimObjectImporter" />
	/// </summary>
	/// <seealso cref="IdeaStatiCa.BimImporter.IBimObjectImporter" />
	public class BimObjectImporter : IBimObjectImporter
	{
		private readonly IPluginLogger _logger;
		private readonly IImporter<IIdeaObject> _importer;
		private readonly IResultImporter _resultImporter;
		private readonly IProgressMessaging _remoteApp;
		private readonly IBimResultsProvider _resultsProvider;
		private readonly BimImporterConfiguration _configuration;

		/// <summary>
		/// Creates instance of <see cref="IBimObjectImporter"/> with specific logger.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="configuration">Importer configuration</param>
		/// <returns>IBimObjectImporter instance.</returns>
		///
		public static IBimObjectImporter Create(
			IPluginLogger logger,
			BimImporterConfiguration configuration,
			IBimResultsProvider resultsProvider,
			IProgressMessaging remoteApp = null /* @Todo: make this mandatory */)
		{
			return new BimObjectImporter(
				logger,
				new ObjectImporter(logger),
				new ResultImporter(logger),
				configuration,
				remoteApp,
				resultsProvider);
		}

		internal BimObjectImporter(
			IPluginLogger logger,
			IImporter<IIdeaObject> importer,
			IResultImporter resultImporter,
			BimImporterConfiguration configuration,
			IProgressMessaging remoteApp,
			IBimResultsProvider resultsProvider)
		{
			_logger = logger;
			_importer = importer;
			_resultImporter = resultImporter;
			_configuration = configuration;
			_remoteApp = remoteApp;
			_resultsProvider = resultsProvider;
		}

		/// <summary>
		/// Converts the objects and bim items into a ModelBIM instance.
		/// </summary>
		/// <param name="objects">Objects to import.</param>
		/// <param name="bimItems">Bim items to import.</param>
		/// <param name="project">Project for storing of mappings and tokens.</param>
		/// <returns>ModelBIM</returns>
		public ModelBIM Import(IEnumerable<IIdeaObject> objects, IEnumerable<IBimItem> bimItems, IProject project, CountryCode countryCode)
		{
			_remoteApp?.SendMessageLocalised(MessageSeverity.Info, LocalisedMessage.ImportStarted);
			ImportContext importContext = new ImportContext(_importer, _resultImporter, project, _logger, _configuration, countryCode);

			if (!(bimItems is null))
			{
				int i = 1;
				int count = bimItems.Count();
				foreach (IBimItem bimItem in bimItems)
				{
					_remoteApp?.SetStageLocalised(i, count, bimItem is Member ? LocalisedMessage.ImportingMembers : LocalisedMessage.ImportingConnections);
					importContext.ImportBimItem(bimItem);
					i++;
				}
			}

			if (!(objects is null))
			{
				_remoteApp?.SendMessageLocalised(MessageSeverity.Info, LocalisedMessage.InternalImport);
				int i = 1;
				int count = objects.Count();
				foreach (IIdeaObject obj in objects)
				{
					_remoteApp?.SetStageLocalised(i, count, LocalisedMessage.ImportingIOMObject);
					importContext.Import(obj);
					i++;
				}
			}

			importContext.ImportResults(_resultsProvider);

			return new ModelBIM()
			{
				Items = importContext.BimItems,
				Messages = new IdeaRS.OpenModel.Message.OpenMessages(),
				Model = importContext.OpenModel,
				Project = "",
				Results = importContext.OpenModelResult
			};
		}
	}
}