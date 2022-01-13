using IdeaStatiCa.RamToIdea.BimApi;
using IdeaStatiCa.RamToIdea.Geometry;
using System.Collections.Generic;

namespace IdeaStatiCa.RamToIdea.Factories
{
	internal interface ISegmentFactory
	{
		List<RamLineSegment3D> CreateSegments(Line line);
	}
}