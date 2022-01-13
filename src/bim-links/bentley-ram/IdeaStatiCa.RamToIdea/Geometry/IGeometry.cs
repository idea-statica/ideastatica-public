using RAMDATAACCESSLib;

namespace IdeaStatiCa.RamToIdea.Geometry
{
	internal interface IGeometry
	{
		Line CreateLine(SCoordinate start, SCoordinate end);

		void AddNode(INode node);
	}
}