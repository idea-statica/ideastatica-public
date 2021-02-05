
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IdeaStatiCa.BimApi
{
	public interface IIdeaMember1D : IIdeaObject
	{

		IdeaRS.OpenModel.Model.Member1DType Type { get; }

		HashSet<IIdeaElement1D> Elements { get; }

		IIdeaNode StartNode { get; }

		IIdeaNode EndNode { get; }

		// IdeaReleases startReleases;

		// IdeaReleases endReleases;

		// void midPoint;

	}
}