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
			if (_refElements.TryGetValue(obj, out ReferenceElement refElm))
			{
				_logger.LogDebug($"Reusing already imported object, open model id '{refElm.Id}'");
				return refElm;
			}

			OpenElementId iomObject = _importer.Import(this, obj);
			iomObject.Id = _project.GetIomId(obj);

			int result = OpenModel.AddObject(iomObject);
			if (result != 0)
			{
				throw new InvalidOperationException($"OpenModel.AddObject failed, return code {result}.");
			}

			refElm = new ReferenceElement(iomObject);
			_refElements.Add(obj, refElm);

			if (obj is IIdeaObjectWithResults objectWithResults)
			{
				_resultOnMembers.Members.AddRange(_resultImporter.Import(this, refElm, objectWithResults));
			}

			return refElm;
		}
	}
}