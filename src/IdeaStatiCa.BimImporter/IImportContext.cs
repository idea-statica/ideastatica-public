using IdeaRS.OpenModel;
using IdeaStatiCa.BimApi;

namespace IdeaStatiCa.BimImporter
{
	internal interface IImportContext
	{
		ReferenceElement Import(IIdeaObject obj);
	}
}