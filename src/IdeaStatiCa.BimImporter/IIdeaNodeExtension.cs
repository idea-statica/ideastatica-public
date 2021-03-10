using IdeaStatiCa.BimApi;
using System;
using MathNet.Numerics;

namespace IdeaStatiCa.BimImporter
{
	internal static class IIdeaNodeExtension
	{
		private const double Precision = 1e-5;

		public static bool IsAlmostEqual(this IIdeaNode node1, IIdeaNode node2)
		{
			if (node1 == node2)
			{
				return true;
			}

			return node1.X.AlmostEqual(node2.X, Precision) &&
				node1.Y.AlmostEqual(node2.Y, Precision) &&
				node1.Z.AlmostEqual(node2.Z, Precision);
		}
	}
}