using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.BimItems;
using IdeaStatiCa.BimImporter.Importers;
using IdeaStatiCa.Plugin;
using System.Collections.Generic;

namespace IdeaStatiCa.BimImporter
{
	public class BimObjectImporter : IBimObjectImporter
	{
		private readonly IPluginLogger _logger;
		private readonly IImporter<IIdeaObject> _importer;
		private readonly IResultImporter _resultImporter;

		public static IBimObjectImporter Create(IPluginLogger logger)
		{
			return new BimObjectImporter(logger, new ObjectImporter(logger), new ResultImporter(logger));
		}

		internal BimObjectImporter(IPluginLogger logger, IImporter<IIdeaObject> importer, IResultImporter resultImporter)
		{
			_logger = logger;
			_importer = importer;
			_resultImporter = resultImporter;
		}

		public ModelBIM Import(IEnumerable<IIdeaObject> objects, IEnumerable<IBimItem> bimItems, IProject project)
		{
			ImportContext importContext = new ImportContext(_importer, _resultImporter, project, _logger);

			foreach (IBimItem bimItem in bimItems)
			{
				importContext.ImportBimItem(bimItem);
			}

			foreach (IIdeaObject obj in objects)
			{
				importContext.Import(obj);
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