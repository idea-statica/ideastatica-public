using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace CI
{
	/// <summary>
	/// Vector extension
	/// </summary>
	public static class VectorExtension
	{
		private const double DegToRad = Math.PI / 180;

		/// <summary>
		/// Rotate vector by degrees
		/// </summary>
		/// <param name="v">Vector</param>
		/// <param name="degrees">Rotated degrees</param>
		/// <returns>Rotated vector</returns>
		public static Vector Rotate(this Vector v, double degrees)
		{
			return v.RotateRadians(degrees * DegToRad);
		}

		/// <summary>
		/// Rotate vector by radians
		/// </summary>
		/// <param name="v">Vector</param>
		/// <param name="radians">Rotated radians</param>
		/// <returns>Rotated vector</returns>
		public static Vector RotateRadians(this Vector v, double radians)
		{
			var ca = Math.Cos(radians);
			var sa = Math.Sin(radians);
			return new Vector(ca * v.X - sa * v.Y, sa * v.X + ca * v.Y);
		}
	}
}
