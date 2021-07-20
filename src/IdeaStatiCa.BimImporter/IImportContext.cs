using IdeaRS.OpenModel;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.BimItems;

namespace IdeaStatiCa.BimImporter
{
	internal interface IImportContext
	{
		ReferenceElement Import(IIdeaObject obj);

		void ImportBimItem(IBimItem bimItem);
	}
}