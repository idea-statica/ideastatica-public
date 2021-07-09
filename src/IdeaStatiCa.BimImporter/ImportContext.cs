using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Result;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.Importers;
using IdeaStatiCa.Plugin;
using System;
using System.Collections.Generic;

namespace IdeaStatiCa.BimImporter
{
	internal class ImportContext : IImportContext
	{
		public OpenModel OpenModel { get; } = new OpenModel();

		public OpenModelResult OpenModelResult { get; } = new OpenModelResult();

		public List<BIMItemId> BimItems { get; } = new List<BIMItemId>();

		private readonly Dictionary<IIdeaObject, ReferenceElement> _refElements
			= new Dictionary<IIdeaObject, ReferenceElement>(new IIdeaObjectComparer());

		private readonly ResultOnMembers _resultOnMembers = new ResultOnMembers();

		private readonly IPluginLogger _logger;
		private readonly IProject _project;
		private readonly IImporter<IIdeaObject> _importer;
		private readonly IResultImporter _resultImporter;

		public ImportContext(IImporter<IIdeaObject> importer, IResultImporter resultImporter, IProject project, IPluginLogger logger)
		{
			_importer = importer;
			_resultImporter = resultImporter;
			_project = project;
			_logger = logger;

			OpenModelResult.ResultOnMembers.Add(_resultOnMembers);
		}

		public ReferenceElement Import(IIdeaObject obj)
		{
			_logger.LogDebug($"Importing object '{obj.Id}', name '{obj.Name}'");

			if (_refElements.TryGetValue(obj, out ReferenceElement refElm))
			{
				_logger.LogDebug($"Object has been already imported with IOM id '{refElm.Id}'");
				return refElm;
			}

			refElm = CreateAndStoreReferenceElement(obj);
			_logger.LogDebug($"Object '{obj.Id}' imported, IOM id '{refElm.Id}'");

			ImportResults(obj, refElm);

			return refElm;
		}

		private void ImportResults(IIdeaObject obj, ReferenceElement refElm)
		{
			if (obj is IIdeaObjectWithResults objectWithResults)
			{
				_logger.LogDebug($"Importing results for object '{obj.Id}'");
				_resultOnMembers.Members.AddRange(_resultImporter.Import(this, refElm, objectWithResults));
			}
		}

		private ReferenceElement CreateAndStoreReferenceElement(IIdeaObject obj)
		{
			OpenElementId iomObject = _importer.Import(this, obj);
			iomObject.Id = _project.GetIomId(obj);

			int result = OpenModel.AddObject(iomObject);
			if (result != 0)
			{
				throw new InvalidOperationException($"OpenModel.AddObject failed, return code '{result}'.");
			}

			ReferenceElement refElm = new ReferenceElement(iomObject);
			_refElements.Add(obj, refElm);

			return refElm;
		}
	}
}