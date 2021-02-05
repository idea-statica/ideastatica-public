
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IdeaStatiCa.BimApi
{
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