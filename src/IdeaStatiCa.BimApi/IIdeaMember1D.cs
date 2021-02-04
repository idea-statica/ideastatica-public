
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IdeaStatiCa.BimApi
{
	public class IIdeaMember1D : IIdeaObject
	{

		public IIdeaMember1D()
		{
		}

		public IdeaRS.OpenModel.Model.Member1DType type;

		public HashSet<IIdeaElement1D> elements;

		public IIdeaNode startNode;

		public IIdeaNode endNode;

		// public IdeaReleases startReleases;

		// public IdeaReleases endReleases;

		// public void midPoint;

	}
}