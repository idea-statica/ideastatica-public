using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Connection;
using IdeaStatiCa.BimApi;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal interface IImporter<T> where T : IIdeaObject
	{
		/// <summary>
		/// Converts an object from BimApi to IOM object and imports it into the OpenModel.
		/// </summary>
		/// <param name="openModel"></param>
		/// <param name="obj"></param>
		/// <returns></returns>
		OpenElementId Import(IImportContext ctx, T obj);


		/// <summary>
		/// Converts an connection object from BimApi to object and imports it into the OpenModel connection data.
		/// </summary>
		/// <param name="openModel"></param>
		/// <param name="obj"></param>
		/// <param name="connectionData"></param>
		/// <returns></returns>
		object Import(IImportContext ctx, T obj, ConnectionData connectionData);
	}
}