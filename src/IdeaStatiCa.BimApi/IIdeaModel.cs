
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IdeaStatiCa.BimApi
{
	public interface IIdeaModel
	{

		/// <summary>
		/// @return
		/// </summary>
		HashSet<IIdeaNode> GetNodes();

		/// <summary>
		/// @return
		/// </summary>
		HashSet<IIdeaMember1D> GetMembers();

		/// <summary>
		/// @return
		/// </summary>
		HashSet<IIdeaMaterial> GetMaterials();

		/// <summary>
		/// @return
		/// </summary>
		IIdeaCrossSection GetCrossSections();
		
		/// <summary>
		/// @param nodes 
		/// @param members
		/// </summary>
		void GetSelection(HashSet<IIdeaNode> nodes, HashSet<IIdeaMember1D> members);

	}
}