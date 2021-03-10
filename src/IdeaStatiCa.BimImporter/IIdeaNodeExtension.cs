using IdeaStatiCa.BimApi;
using System;

namespace IdeaStatiCa.BimImporter
{
	internal static class IIdeaNodeExtension
	{
		private const double Precision = 0.0001;

		public static bool IsSimilarTo(this IIdeaNode node1, IIdeaNode node2, double epsilon = Precision)
		{
			if (node1 == node2)
			{
				return true;
			}

			return Math.Abs(node1.X - node2.X) <= epsilon &&
				Math.Abs(node1.Y - node2.Y) <= epsilon &&
				Math.Abs(node1.Z - node2.Z) <= epsilon;
		}
	}
}