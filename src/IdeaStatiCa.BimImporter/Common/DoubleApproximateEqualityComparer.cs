using IdeaStatiCa.BimImporter.Extensions;
using MathNet.Numerics;
using System.Collections.Generic;

namespace IdeaStatiCa.BimImporter.Common
{
	/// <summary>
	/// Equality compare for doubles with defined precision.
	/// </summary>
	public class DoubleApproximateEqualityComparer : IEqualityComparer<double>
	{
		/// <summary>
		/// Precision of this comparer.
		/// </summary>
		public double Precision
		{
			get => _precision;
			set
			{
				_precision = value;
				_leadingZeros = value.LeadingDecimalZeros();
			}
		}

		private double _precision;
		private int _leadingZeros;

		/// <summary>
		/// Creates an instance of <see cref="DoubleApproximateEqualityComparer"/>.
		/// </summary>
		/// <param name="precision">Precision of the comparer.</param>
		public DoubleApproximateEqualityComparer(double precision = double.Epsilon)
		{
			Precision = precision;
		}

		/// <summary>
		/// Determines whether two numbers are equal within given <see cref="Precision"/>.
		/// For example, if the precision is 0.1 then numbers 0.1 and 0.12 are equal but
		/// numbers 0.1 and 0.2 are not.
		/// </summary>
		/// <param name="x">First number to compare.</param>
		/// <param name="y">Second number to compare.</param>
		/// <returns>True if numbers are equal within given <see cref="Precision"/>, false otherwise.</returns>
		public bool Equals(double x, double y)
		{
			return x.AlmostEqual(y, _precision);
		}

		/// <summary>
		/// Returns a hash code of the number with respect to the <see cref="Precision"/>.
		/// For example, if the precision is 0.1 then number 0.1 and 0.12 will have the same hash code but
		/// number 0.1 and 0.2 will not.
		/// </summary>
		/// <param name="x">Number for which a hash code is to be returned.</param>
		/// <returns></returns>
		public int GetHashCode(double x)
		{
			return x.Round(_leadingZeros).GetHashCode();
		}
	}
}