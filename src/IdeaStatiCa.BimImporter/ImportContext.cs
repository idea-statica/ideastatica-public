using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Connection;
using IdeaRS.OpenModel.Result;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.BimItems;
using IdeaStatiCa.BimImporter.Importers;
using IdeaStatiCa.Plugin;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace IdeaStatiCa.BimImporter
{
	internal class ImportContext : IImportContext
	{
		public OpenModel OpenModel { get; } = new OpenModel();

		public OpenModelResult OpenModelResult { get; } = new OpenModelResult();

		public List<BIMItemId> BimItems { get; } = new List<BIMItemId>();

		public BimImporterConfiguration Configuration { get; private set; }

		private readonly Dictionary<IIdeaObject, ReferenceElement> _refElements
			= new Dictionary<IIdeaObject, ReferenceElement>(new IIdeaObjectComparer());

		private readonly Dictionary<IIdeaObject, object> _refConnectionItems
			= new Dictionary<IIdeaObject, object>(new IIdeaObjectComparer());

		private readonly ResultOnMembers _resultOnMembers = new ResultOnMembers();

		private readonly IPluginLogger _logger;
		private readonly IProject _project;
		private readonly IImporter<IIdeaObject> _importer;
		private readonly IResultImporter _resultImporter;

		public ImportContext(IImporter<IIdeaObject> importer, IResultImporter resultImporter, IProject project, IPluginLogger logger,
			BimImporterConfiguration configuration)
		{
			_importer = importer;
			_resultImporter = resultImporter;
			_project = project;
			_logger = logger;

			Configuration = configuration;

			OpenModelResult.ResultOnMembers.Add(_resultOnMembers);
		}

		public ReferenceElement Import(IIdeaObject obj)
		{
			_logger.LogDebug($"Importing object '{obj.Id}', name '{obj.Name}'");

			if (_refElements.TryGetValue(obj, out ReferenceElement refElm))
			{
				_logger.LogTrace($"Object has been already imported with IOM id '{refElm.Id}'");
				return refElm;
			}

			refElm = CreateAndStoreReferenceElement(obj);
			Debug.Assert(_refElements[obj] == refElm);

			_logger.LogTrace($"Object '{obj.Id}' imported, IOM id '{refElm.Id}'");

			ImportResults(obj, refElm);

			return refElm;
		}

		public void ImportBimItem(IBimItem bimItem)
		{
			Debug.Assert(bimItem != null);

			ReferenceElement refElm = Import(bimItem.ReferencedObject);
			BimItems.Add(new BIMItemId()
			{
				Type = bimItem.Type,
				Id = refElm.Id
			});
		}

		private void ImportResults(IIdeaObject obj, ReferenceElement refElm)
		{
			if (obj is IIdeaObjectWithResults objectWithResults)
			{
				_logger.LogTrace($"Importing results for object '{obj.Id}'");
				_resultOnMembers.Members.AddRange(_resultImporter.Import(this, refElm, objectWithResults));
			}
		}

		private ReferenceElement CreateAndStoreReferenceElement(IIdeaObject obj)
		{
			OpenElementId iomObject = _importer.Import(this, obj);
			Debug.Assert(iomObject != null);

			iomObject.Id = _project.GetIomId(obj);

			int result = OpenModel.AddObject(iomObject);
			if (result != 0)
			{
				//skip object witch is not in IOM collection
				if (result == -10)
				{
					_logger.LogDebug($"OpenModel.AddObject skiped adding to the collection, return code '{result}'. Due");
				}
				else
				{
					throw new InvalidOperationException($"OpenModel.AddObject failed, return code '{result}'.");
				}
			}

			ReferenceElement refElm = new ReferenceElement(iomObject);
			_refElements.Add(obj, refElm);

			if (iomObject is ConnectionPoint cp && OpenModel.Connections.Count > 0)
			{
				OpenModel.Connections[OpenModel.Connections.Count - 1].ConenctionPointId = cp.Id;
			}

			return refElm;
		}

		public object ImportConnectionItem(IIdeaObject obj, ConnectionData connectionData)
		{
			if (_refConnectionItems.TryGetValue(obj, out object refElm))
			{
				_logger.LogDebug($"Object has been already imported with IOM id '{refElm}'");
				return refElm;
			}

			var item = _importer.Import(this, obj, connectionData);
			if (item == null)
			{
				throw new InvalidOperationException($"OpenModel add connection item failed, return code '{item}'.");
			}
			_refConnectionItems.Add(obj, item);

			return item;
		}
	}
}