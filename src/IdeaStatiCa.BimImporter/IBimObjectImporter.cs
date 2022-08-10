using IdeaRS.OpenModel;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.BimItems;
using IdeaStatiCa.Plugin;
using System.Collections.Generic;

namespace IdeaStatiCa.BimImporter
{
	/// <summary>
	/// Interface IBimObjectImporter
	/// </summary>
	public interface IBimObjectImporter
	{
		/// <summary>
		/// Import objects and bim items into IOM.
		/// </summary>
		/// <param name="objects">Objects to import.</param>
		/// <param name="bimItems">Bim items to import.</param>
		/// <param name="project">Project for storing of mappings and tokens.</param>
		/// <param name="countryCode">Country code.</param>
		/// <returns>ModelBIM</returns>
		ModelBIM Import(IEnumerable<IIdeaObject> objects, IEnumerable<IBimItem> bimItems, IProject project, CountryCode countryCode);
	}
}