using IdeaRS.OpenModel;
using IdeaStatiCa.BimApi;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal interface IImporter<in T> where T : IIdeaObject
	{
		/// <summary>
		/// Converts an object from BimApi to IOM object and imports it into the OpenModel.
		/// </summary>
		/// <param name="ctx">Importer context</param>
		/// <param name="obj">Object to import</param>
		/// <returns>IOM object</returns>
		OpenElementId Import(IImportContext ctx, T obj);
	}
}