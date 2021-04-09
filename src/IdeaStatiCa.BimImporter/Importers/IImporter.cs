using IdeaRS.OpenModel;
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
		OpenElementId Import(ImportContext ctx, T obj);
	}
}