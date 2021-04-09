using IdeaRS.OpenModel;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.Importers;
using IdeaStatiCa.Diagnostics;
using System;
using System.Collections.Generic;

namespace IdeaStatiCa.BimImporter
{
	internal class ImportContext
	{
		private readonly static IIdeaLogger _logger = IdeaDiagnostics.GetLogger("ideastatica.bimimporter.importcontext");

		public Dictionary<IIdeaObject, ReferenceElement> ReferenceElements { get; }
			= new Dictionary<IIdeaObject, ReferenceElement>(new IIdeaObjectComparer());

		public OpenModel OpenModel { get; } = new OpenModel();

		public IImporter<IIdeaObject> Importer { get; }

		public Project Project { get; }

		public ImportContext(IImporter<IIdeaObject> importer, Project project)
		{
			Importer = importer;
			Project = project;
		}

		public ReferenceElement Import(IIdeaObject obj)
		{
			if (ReferenceElements.TryGetValue(obj, out ReferenceElement refElm))
			{
				_logger.LogDebug($"Reusing already imported object, open model id '{refElm.Id}'");
				return refElm;
			}

			OpenElementId iomObject = Importer.Import(this, obj);
			iomObject.Id = Project.GetIomId(obj);

			int result = OpenModel.AddObject(iomObject);
			if (result != 0)
			{
				throw new Exception();
			}

			refElm = new ReferenceElement(iomObject);
			ReferenceElements.Add(obj, refElm);

			return refElm;
		}
	}
}