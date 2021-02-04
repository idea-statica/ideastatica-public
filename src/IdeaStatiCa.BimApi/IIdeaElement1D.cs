
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IdeaStatiCa.BimApi
{
	public class IIdeaElement1D : IIdeaObject
	{

		public IIdeaElement1D()
		{
		}

		public IIdeaNode startNode;

		public IIdeaNode endNode;

		public IIdeaCrossSection startCrossSection;

		public IIdeaCrossSection endCrossSection;

		public double excentricityBegin;

		public double excentricityEnd;

		public double rotationRx;

		public IIdeaSegment3D segment;


	}
}