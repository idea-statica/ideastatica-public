using IdeaRS.OpenModel;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.Importers;
using IdeaStatiCa.Plugin;
using System;
using System.Collections.Generic;

namespace IdeaStatiCa.BimImporter
{
	internal class ImportContext : IImportContext
	{
		private readonly IPluginLogger _logger;

		public OpenModel OpenModel { get; } = new OpenModel();

		public IImporter<IIdeaObject> Importer { get; }

		public IProject Project { get; }

		private readonly Dictionary<IIdeaObject, ReferenceElement> _refElements
			= new Dictionary<IIdeaObject, ReferenceElement>(new IIdeaObjectComparer());

		public ImportContext(IImporter<IIdeaObject> importer, IProject project, IPluginLogger logger)
		{
			Importer = importer;
			Project = project;
			_logger = logger;
		}

		public ReferenceElement Import(IIdeaObject obj)
		{
			if (_refElements.TryGetValue(obj, out ReferenceElement refElm))
			{
				_logger.LogDebug($"Reusing already imported object, open model id '{refElm.Id}'");
				return refElm;
			}

			OpenElementId iomObject = Importer.Import(this, obj);
			iomObject.Id = Project.GetIomId(obj);

			int result = OpenModel.AddObject(iomObject);
			if (result != 0)
			{
				throw new InvalidOperationException($"OpenModel.AddObject failed, return code {result}.");
			}

			refElm = new ReferenceElement(iomObject);
			_refElements.Add(obj, refElm);

			return refElm;
		}
	}
}