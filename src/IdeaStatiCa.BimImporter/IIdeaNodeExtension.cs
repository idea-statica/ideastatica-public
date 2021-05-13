using IdeaStatiCa.BimApi;
using MathNet.Numerics;

namespace IdeaStatiCa.BimImporter
{
	internal static class IIdeaNodeExtension
	{
		public static bool IsAlmostEqual(this IIdeaNode node1, IIdeaNode node2)
		{
			if (node1 == node2)
			{
				return true;
			}

			IdeaVector3D vec1 = node1.Vector;
			IdeaVector3D vec2 = node2.Vector;

			return vec1.X.AlmostEqual(vec2.X, Constants.Precision) &&
				vec1.Y.AlmostEqual(vec2.Y, Constants.Precision) &&
				vec1.Z.AlmostEqual(vec2.Z, Constants.Precision);
		}
	}
}