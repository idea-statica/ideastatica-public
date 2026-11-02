using System;

namespace CI.Geometry2D
{
	/// <summary>
	/// The description of intersection of two lines.
	/// </summary>
	[Flags]
	public enum IntersectionInfo
	{
		/// <summary>
		/// The lines are collinear.
		/// </summary>
		NoIntersection = -1,

		/// <summary>
		/// The intersection is inside both lines.
		/// </summary>
		IntersectionInside = 1,

		/// <summary>
		/// The intersection is inside first and outside second line.
		/// </summary>
		IntersectionFirst = 2,

		/// <summary>
		/// The intersection is outside first and inside second line.
		/// </summary>
		IntersectionSecond = 4,

		/// <summary>
		/// The intersection is outside both lines.
		/// </summary>
		IntersectionOutside = 8,

		/// <summary>
		/// Any intersection.
		/// </summary>
		Intersection = IntersectionInside | IntersectionFirst | IntersectionSecond | IntersectionOutside,
	}

	internal static class IntersectionInfoHelper
	{
		public static bool IsSubset(IntersectionInfo part, IntersectionInfo entire)
		{
			return (part | entire) == entire;
		}
	}
}