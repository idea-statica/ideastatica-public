using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Result;
using IdeaStatiCa.BimApi;
using System.Collections.Generic;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal interface IResultImporter
	{
		IEnumerable<ResultOnMember> Import(IImportContext ctx, ReferenceElement referenceElement, IIdeaObjectWithResults obj);
	}
}