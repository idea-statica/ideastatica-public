using IdeaStatiCa.Plugin;
using System.Collections.Generic;

namespace IdeaStatiCa.BimImporter
{
	/// <summary>
	/// 
	/// </summary>
	public interface IBimImporter
	{
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		ModelBIM ImportConnections();

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		ModelBIM ImportMembers();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="selected"></param>
		/// <returns></returns>
		List<ModelBIM> ImportSelected(List<BIMItemsGroup> selected);
	}
}