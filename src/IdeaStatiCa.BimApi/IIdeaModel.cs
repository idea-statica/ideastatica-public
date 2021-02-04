
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IdeaStatiCa.BimApi
{
	public abstract class IIdeaModel
	{

		public IIdeaModel()
		{
		}







		/// <summary>
		/// @return
		/// </summary>
		public abstract HashSet<IIdeaNode> GetNodes();

		/// <summary>
		/// @return
		/// </summary>
		public HashSet<IIdeaMember1D> GetMembers()
		{
			// TODO implement here
			return null;
		}

		/// <summary>
		/// @return
		/// </summary>
		public HashSet<IIdeaMaterial> GetMaterials()
		{
			// TODO implement here
			return null;
		}

		/// <summary>
		/// @return
		/// </summary>
		public IIdeaCrossSection GetCrossSections()
		{
			// TODO implement here
			return null;
		}

		/// <summary>
		/// @param nodes 
		/// @param members
		/// </summary>
		public abstract void GetSelection(HashSet<IIdeaNode> nodes, HashSet<IIdeaMember1D> members);

	}
}