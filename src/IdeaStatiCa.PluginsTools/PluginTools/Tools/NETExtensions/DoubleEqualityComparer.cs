using System;
using System.Collections.Generic;

namespace CI
{
	/// <summary>
	/// Defines methods to support the comparison of two double values for equality with tolerance.
	/// </summary>
	public class DoubleEqualityComparer : IEqualityComparer<double>
	{
		private double tolerance = 1e-9;

		/// <summary>
		/// Sets the tolerance for comaprison of two double values.
		/// </summary>
		public double Tolerance
		{
			set { tolerance = value; }
		}

		/// <summary>
		/// Determines whether the specified double values are equal.
		/// </summary>
		/// <param name="x">The first value to compare.</param>
		/// <param name="y">The second value to compare.</param>
		/// <returns>True if the specified values are equal with specified tolerance; otherwise, false.</returns>
		public bool Equals(double x, double y)
		{
			return x.IsEqual(y, tolerance);
		}

		/// <summary>
		/// Returns a hash code for the specified value.
		/// </summary>
		/// <param name="obj">The value for which a hash code is to be returned.</param>
		/// <returns>A hash code for the specified value.</returns>
		public int GetHashCode(double obj)
		{
			return (int)Math.Round(obj / tolerance);
		}
	}
}
