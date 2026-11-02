using System.Collections.Generic;

namespace IdeaStatiCa.BimApi
{
	/// <summary>
	/// TODO
	/// </summary>
	public interface IIdeaMaterialLibrary
	{
		/// <summary>
		/// @return
		/// </summary>
		HashSet<string> GetMaterialNames();

		/// <summary>
		/// @param materialName
		/// @return
		/// </summary>
		bool IsValidIdeaMaterialName(string materialName);
	}
}