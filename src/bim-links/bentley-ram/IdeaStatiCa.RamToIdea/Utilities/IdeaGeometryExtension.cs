using IdeaStatiCa.BimApi;
using MathNet.Spatial.Euclidean;

namespace IdeaStatiCa.RamToIdea.Utilities
{
	internal static class IdeaGeometryExtension
	{
		public static Vector3D ToMNVector(this IIdeaNode node)
		{
			return node.Vector.ToMNVector();
		}

		public static Vector3D ToMNVector(this IdeaVector3D v)
		{
			return new Vector3D(v.X, v.Y, v.Z);
		}
	}
}