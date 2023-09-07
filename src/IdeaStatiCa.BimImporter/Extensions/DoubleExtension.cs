using System;

namespace IdeaStatiCa.BimImporter.Extensions
{
	public static class DoubleExtension
	{
		/// <summary>
		/// Computes number of leading zeros for a decimal number. For example, for 0.002 it returns 3.
		/// </summary>
		/// <param name="value">Number for which a number of leading zeros is to be calculated.</param>
		/// <returns>Number of leading zeros.</returns>
		public static int LeadingDecimalZeros(this double value)
		{
			return (int)-Math.Log10(value);
		}

		internal static double tolerance = 1e-9;

		public static bool IsZero(this double number)
		{
			return Math.Abs(number) < tolerance;
		}

		public static bool IsZero(this double number, double tolerance)
		{
			return Math.Abs(number) < tolerance;
		}

		/// <summary>
		/// IsEqual - Determines whether leftValue and rightValue are equal.
		/// </summary>
		/// <param name="leftValue">Value on LHS of operator</param>
		/// <param name="rightValue">Value on RHS of operator</param>
		/// <param name="tolerance">Tolerance level for comparison</param>
		/// <returns>Return true if leftValue and rightValue are equal. Return false otherwise</returns>
		public static bool IsEqual(this double leftValue, double rightValue, double tolerance = 1e-10)
		{
			if ((double.IsPositiveInfinity(leftValue) && double.IsPositiveInfinity(rightValue))
				|| (double.IsNegativeInfinity(leftValue) && double.IsNegativeInfinity(rightValue)))
			{
				return true;
			}

			return Math.Abs(leftValue - rightValue) <= tolerance;
		}
	}
}