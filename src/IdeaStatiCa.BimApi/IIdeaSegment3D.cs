
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IdeaStatiCa.BimApi
{
	public interface IIdeaSegment3D : IIdeaObject
	{

		IIdeaNode StartNode { get; }

		IIdeaNode EndNode { get; }

		IdeaRS.OpenModel.Geometry3D.CoordSystem LocalCoordinateSystem { get; }

	}
}