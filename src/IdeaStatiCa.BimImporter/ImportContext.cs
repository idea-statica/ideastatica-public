using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Connection;
using IdeaRS.OpenModel.Result;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.BimItems;
using IdeaStatiCa.BimImporter.Importers;
using IdeaStatiCa.BimImporter.Results;
using IdeaStatiCa.Plugin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace IdeaStatiCa.BimImporter
{
	internal class ImportContext : IImportContext
	{
		public OpenModel OpenModel { get; } = new OpenModel();

		public OpenModelResult OpenModelResult { get; } = new OpenModelResult();

		public List<BIMItemId> BimItems { get; } = new List<BIMItemId>();

		public CountryCode CountryCode { get; }

		public BimImporterConfiguration Configuration { get; }

		private readonly Dictionary<IIdeaObject, ReferenceElement> _refElements
			= new Dictionary<IIdeaObject, ReferenceElement>(new IIdeaObjectComparer());

		private readonly Dictionary<IIdeaObject, object> _refConnectionItems
			= new Dictionary<IIdeaObject, object>(new IIdeaObjectComparer());

		private readonly List<IIdeaObjectWithResults> _objectsWithResults
			= new List<IIdeaObjectWithResults>();

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
		}

		public ImportContext(IImporter<IIdeaObject> importer, IResultImporter resultImporter, IProject project, IPluginLogger logger,
			BimImporterConfiguration configuration, CountryCode countryCode)
		{
			_importer = importer;
			_resultImporter = resultImporter;
			_project = project;
			_logger = logger;

			Configuration = configuration;

			CountryCode = countryCode;
		}

		public ReferenceElement Import(IIdeaObject obj)
		{
			if (obj is null)
			{
				_logger.LogTrace($"Trying to import null object.");
				return null;
			}

			if (_refElements.TryGetValue(obj, out ReferenceElement refElm))
			{
				_logger.LogTrace($"Object has been already imported with IOM id '{refElm.Id}'");
				return refElm;
			}

			_logger.LogDebug($"Importing object '{obj.Id}', name '{obj.Name}'");

			refElm = CreateAndStoreReferenceElement(obj);
			Debug.Assert(_refElements[obj] == refElm);

			_logger.LogTrace($"Object '{obj.Id}' imported, IOM id '{refElm.Id}'");

			PrepareToImportResults(obj);

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

		public void ImportResults(IBimResultsProvider resultsProvider)
		{
			Debug.Assert(OpenModelResult.ResultOnMembers.Count == 0);

			ResultOnMembers resultOnMembers = new ResultOnMembers();

			int count = 0;

			// Call into Import may add objects _objectsWithResults
			// so we need to make sure to process them.
			while (_objectsWithResults.Count - count > 0)
			{
				foreach (ResultsData data in resultsProvider.GetResults(_objectsWithResults.Skip(count)))
				{
					ReferenceElement refElm = Import(data.Object);

					IEnumerable<ResultOnMember> results = _resultImporter.Import(this, refElm, data);

					// for all new results we need to know, if they are in the list of already imported results
					foreach (var resultOnMember in results)
					{
						// if results on the given member exists, we will add new data to them, else we will add new member results
						ResultOnMember existingResultOnMember = resultOnMembers.Members.FirstOrDefault(r => resultOnMember.Member.Id == r.Member.Id);
						if (existingResultOnMember != null)
						{
							MergeResultsOnMember(existingResultOnMember, resultOnMember);
						}
						else
						{
							resultOnMembers.Members.Add(resultOnMember);
						}
					}
				}

				count = _objectsWithResults.Count;
			}

			OpenModelResult.ResultOnMembers.Add(resultOnMembers);
		}

		private static void MergeResultsOnMember(ResultOnMember existingResultOnMember, ResultOnMember resultOnMember)
		{
			// we have to pair sections with existing sections
			foreach (ResultOnSection newResultOnMember in resultOnMember.Results.Cast<ResultOnSection>())
			{
				var existingSectionResultOnMember = existingResultOnMember.Results.OfType<ResultOnSection>().FirstOrDefault(x => x.Position == newResultOnMember.Position);
				if (existingSectionResultOnMember is null)
				{
					// add new section with all load cases
					existingResultOnMember.Results.Add(newResultOnMember);
				}
				else
				{
					// add new results in all load cases to the existing section
					existingSectionResultOnMember.Results.AddRange(newResultOnMember.Results);
				}
			}
		}

		private void PrepareToImportResults(IIdeaObject obj)
		{
			if (obj is IIdeaObjectWithResults objectWithResults)
			{
				_objectsWithResults.Add(objectWithResults);
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
				//skip object which is not in IOM collection
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
				OpenModel.Connections[OpenModel.Connections.Count - 1].ConnectionPoint = new ReferenceElement(cp);
				if (string.IsNullOrWhiteSpace(cp.Name))
				{
					cp.Name = $"C {cp.Id}";
				}
			}

			return refElm;
		}

		public object ImportConnectionItem(IIdeaObject obj, ConnectionData connectionData)
		{
			object item = _importer.Import(this, obj, connectionData);
			if (item == null)
			{
				throw new InvalidOperationException($"OpenModel add connection item failed, return code '{item}'.");
			}

			if (!_refConnectionItems.ContainsKey(obj))
			{
				_refConnectionItems.Add(obj, item);
			}
			return item;
		}
	}
}