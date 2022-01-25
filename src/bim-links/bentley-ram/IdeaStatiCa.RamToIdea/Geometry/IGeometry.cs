using IdeaStatiCa.RamToIdea.BimApi;
using RAMDATAACCESSLib;

namespace IdeaStatiCa.RamToIdea.Geometry
{
	internal interface IGeometry
	{
		Line CreateLine(SCoordinate start, SCoordinate end, bool allowsIntermediateNodes);

		void AddNode(INode node);

		void AddNodeToLine(Line line, SCoordinate position);
	}
}