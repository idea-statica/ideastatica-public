
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IdeaStatiCa.BimApi
{
	public class IIdeaNode : IIdeaObject {

		public IIdeaNode() {
		}

		public double x;

		public double y;

		public double z;

		//public enum degreeOfFreedom;


		/// <summary>
		/// @return
		/// </summary>
		public HashSet<IIdeaMember1D> GetConnectedMembers() {
			// TODO implement here
			return null;
		}

	}
}