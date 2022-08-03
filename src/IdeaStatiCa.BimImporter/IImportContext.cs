using IdeaRS.OpenModel;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.BimItems;

namespace IdeaStatiCa.BimImporter
{
	internal interface IImportContext
	{
		BimImporterConfiguration Configuration { get; }

		CountryCode CoutryCode { get; }

		ReferenceElement Import(IIdeaObject obj);

		void ImportBimItem(IBimItem bimItem);
	}
}