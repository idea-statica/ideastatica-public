using CI.Geometry3D;
using System;
using System.Collections.Generic;

namespace IdeaStatiCa.TeklaStructuresPlugin.Utils
{
	public static class PlateContour
	{
		/// <summary>
		/// Tolerance for treating two loop vertices as one point, in millimetres - the unit Tekla
		/// reports solid geometry in.
		/// </summary>
		private const double CoincidentVertexToleranceMm = 1E-04;

		/// <summary>
		/// Returns the first closed contour in a sequence of vertices.
		/// <para>
		/// A Tekla contour can describe the same outline more than once, revisiting vertices it has
		/// already reported - both a contour plate's own contour points and the face loop built from
		/// them. Such a sequence closes on itself and describes no single region, so only the first
		/// closed cycle is kept. A sequence that visits every vertex once is returned unchanged.
		/// </para>
		/// </summary>
		public static IReadOnlyList<IPoint3D> FirstClosedContour(IReadOnlyList<IPoint3D> vertices)
		{
			FindFirstClosedContour(vertices, out int start, out int count);
			if (count == vertices.Count)
			{
				return vertices;
			}

			var contour = new List<IPoint3D>(count);
			for (int k = start; k < start + count; k++)
			{
				contour.Add(vertices[k]);
			}

			return contour;
		}

		/// <summary>
		/// Locates the first closed contour as a range into <paramref name="vertices"/>, for callers
		/// that hold their vertices in some other type and have to slice that list themselves.
		/// <para>
		/// The repeat is matched against every earlier vertex, not against the first one: nothing
		/// promises a particular start vertex, so the repeated pair can straddle any rotation of the
		/// sequence.
		/// </para>
		/// </summary>
		public static void FindFirstClosedContour(IReadOnlyList<IPoint3D> vertices, out int start, out int count)
		{
			for (int j = 1; j < vertices.Count; j++)
			{
				// A cycle needs at least three vertices, so a repeat closer than that is not one.
				for (int i = 0; i <= j - 3; i++)
				{
					if (IsCoincident(vertices[i], vertices[j]))
					{
						start = i;
						count = j - i;
						return;
					}
				}
			}

			start = 0;
			count = vertices.Count;
		}

		private static bool IsCoincident(IPoint3D first, IPoint3D second)
		{
			return Math.Abs(first.X - second.X) <= CoincidentVertexToleranceMm
				&& Math.Abs(first.Y - second.Y) <= CoincidentVertexToleranceMm
				&& Math.Abs(first.Z - second.Z) <= CoincidentVertexToleranceMm;
		}
	}
}
