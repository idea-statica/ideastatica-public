using MathNet.Spatial.Euclidean;
using RAMDATAACCESSLib;

namespace IdeaStatiCa.RamToIdea.Geometry
{
	public static class SCoordinateExtensions
	{
		public static Vector3D ToVector3D(this SCoordinate coord)
		{
			return new Vector3D(coord.dXLoc, coord.dYLoc, coord.dZLoc);
		}
	}
}