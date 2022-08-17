using IdeaRS.OpenModel;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.BimItems;
using IdeaStatiCa.BimImporter.Importers;
using IdeaStatiCa.Plugin;
using System.Collections.Generic;

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

		private readonly BimImporterConfiguration _configuration;

		/// <summary>
		/// Creates instance of <see cref="IBimObjectImporter"/> with specific logger.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="configuration">Importer configuration</param>
		/// <returns>IBimObjectImporter instance.</returns>
		public static IBimObjectImporter Create(IPluginLogger logger,
			BimImporterConfiguration configuration)
		{
			return new BimObjectImporter(
				logger,
				new ObjectImporter(logger),
				new ResultImporter(logger),
				configuration);
		}

		internal BimObjectImporter(IPluginLogger logger, IImporter<IIdeaObject> importer, IResultImporter resultImporter,
			BimImporterConfiguration configuration)
		{
			_logger = logger;
			_importer = importer;
			_resultImporter = resultImporter;
			_configuration = configuration;
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
			ImportContext importContext = new ImportContext(_importer, _resultImporter, project, _logger, _configuration, countryCode);

			if (!(bimItems is null))
			{
				foreach (IBimItem bimItem in bimItems)
				{
					importContext.ImportBimItem(bimItem);
				}
			}

			if (!(objects is null))
			{
				foreach (IIdeaObject obj in objects)
				{
					importContext.Import(obj);
				}
			}

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