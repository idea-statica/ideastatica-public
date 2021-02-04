
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IdeaStatiCa.BimApi
{
	public class IIdeaSegment3D : IIdeaObject
	{

		public IIdeaSegment3D()
		{
		}

		public IIdeaNode startNode;

		public IIdeaNode endNode;

		public IdeaRS.OpenModel.Geometry3D.CoordSystem localCoordinateSystem;

	}
}