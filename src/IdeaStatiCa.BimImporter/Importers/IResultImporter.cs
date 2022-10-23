using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Result;
using IdeaStatiCa.BimImporter.Results;
using System.Collections.Generic;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal interface IResultImporter
	{
		IEnumerable<ResultOnMember> Import(IImportContext ctx, ReferenceElement referenceElement, ResultsData resultsData);
	}
}