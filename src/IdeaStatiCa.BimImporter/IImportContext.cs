using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Connection;
using IdeaRS.OpenModel.Result;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.BimItems;
using IdeaStatiCa.BimImporter.Results;

namespace IdeaStatiCa.BimImporter
{
	internal interface IImportContext
	{
		OpenModel OpenModel { get; }

		OpenModelResult OpenModelResult { get; }

		BimImporterConfiguration Configuration { get; }

		CountryCode CountryCode { get; }

		ReferenceElement Import(IIdeaObject obj);

		object ImportConnectionItem(IIdeaObject obj, ConnectionData connectionData);

		void ImportBimItem(IBimItem bimItem);

		void ImportResults(IBimResultsProvider resultsProvider);
	}
}