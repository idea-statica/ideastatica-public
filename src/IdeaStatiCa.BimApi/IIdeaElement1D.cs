
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IdeaStatiCa.BimApi
{
	public interface IIdeaElement1D : IIdeaObject
	{

		IIdeaNode StartNode { get; }

		IIdeaNode EndNode { get; }

		IIdeaCrossSection StartCrossSection { get; }

		IIdeaCrossSection EndCrossSection { get; }

		double ExcentricityBegin { get; }

		double ExcentricityEnd { get; }

		double RotationRx { get; }

		IIdeaSegment3D Segment { get; }


	}
}