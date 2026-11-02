using System.Windows;

namespace CI
{
	/// <summary>
	/// Provides an extensions of System.Windows
	/// </summary>
	public static class WindowsExtension
	{
		/// <summary>
		/// Calculates perpendicular vector on the right side.
		/// </summary>
		/// <param name="vector">The source vector.</param>
		/// <returns>The perpendicular vector to the source vector.</returns>
		public static Vector Perpendicular(this Vector vector)
		{
			return new Vector(vector.Y, -vector.X);
		}

		public static bool IsEquals(this Vector vector1, Vector vector2, double tolerance = 1e-12)
		{
#if DEBUG
			var x = vector1.X.IsEqual(vector2.X, tolerance);
			var y = vector1.Y.IsEqual(vector2.Y, tolerance);
#endif
			return vector1.X.IsEqual(vector2.X, tolerance) && vector1.Y.IsEqual(vector2.Y, tolerance);
		}
	}
}
