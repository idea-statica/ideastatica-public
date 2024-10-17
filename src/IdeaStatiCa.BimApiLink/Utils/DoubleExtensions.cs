using System;
using System.Runtime.CompilerServices;

namespace IdeaStatiCa.BimApiLink.Utils
{
	public static class DoubleExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double PrecentageToRadians(this double val)
		{
			return val * 0.06283;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double MetersToMilimeters(this double val)
		{
			return val / 1e-3;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double MilimetersToMeters(this double val)
		{
			return val * 1e-3;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double InchesToMeters(this double val)
		{
			return val * 0.0254;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double MetersToInches(this double val)
		{
			return val / 0.0254;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string MetersToInchesFormated(this double meters)
		{
			// Conversion factor
			double inches = meters * 39.3701;

			// Get the whole number of inches (before the decimal)
			int wholeInches = (int)inches;

			// Convert the decimal portion to fractions for an imperial style
			double fractionalPart = inches - wholeInches;
			string fraction = ConvertDecimalToFraction(fractionalPart);

			return $"{wholeInches}\" {fraction}";
		}

		// Optional: Helper method to convert decimal to a simple fraction (approximated for common use)
		private static string ConvertDecimalToFraction(double decimalPart)
		{
			if (decimalPart == 0) return string.Empty;
			// Common fraction approximations
			if (Math.Abs(decimalPart - 0.125) < 0.01) return "1/8";
			if (Math.Abs(decimalPart - 0.25) < 0.01) return "1/4";
			if (Math.Abs(decimalPart - 0.375) < 0.01) return "3/8";
			if (Math.Abs(decimalPart - 0.5) < 0.01) return "1/2";
			if (Math.Abs(decimalPart - 0.625) < 0.01) return "5/8";
			if (Math.Abs(decimalPart - 0.75) < 0.01) return "3/4";
			if (Math.Abs(decimalPart - 0.875) < 0.01) return "7/8";
			return string.Empty; // Default if no match
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double KipsToNewtons(this double val)
		{
			return val * 4448.2216;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double KipsToMPascal(this double val)
		{
			return val * 6.89475728;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double KipsToKgPerCm2(this double val)
		{
			return val * 70.306957829636;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double DegreesToRadians(this double val)
		{
			return val / 180.0 * Math.PI;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double RadiansToDegrees(this double val)
		{
			return val * 180.0 / Math.PI;
		}

		/// <summary>
		/// IsGreater - Determines whether the leftValue is greater than rightValue
		/// </summary>
		/// <param name="leftValue">Value on LHS of operator</param>
		/// <param name="rightValue">Value on RHS of operator</param>
		/// <param name="tolerance">Tolerance level for comparison</param>
		/// <returns>Return true if the given leftValue is greater than rightValue. Return false otherwise</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsGreater(this double leftValue, double rightValue, double tolerance = 1e-10)
		{
			return (leftValue - rightValue) >= tolerance;
		}

		/// <summary>
		/// IsGreaterOrEqual - Determines whether the leftValue is greater or equal to rightValue
		/// </summary>
		/// <param name="leftValue">Value on LHS of operator</param>
		/// <param name="rightValue">Value on RHS of operator</param>
		/// <param name="tolerance">Tolerance level for comparison</param>
		/// <returns>Return true if leftValue is greater than or equal to rightValue. Return false otherwise</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsGreaterOrEqual(this double leftValue, double rightValue, double tolerance = 1e-10)
		{
			if ((double.IsPositiveInfinity(leftValue) && double.IsPositiveInfinity(rightValue)) ||
				(double.IsNegativeInfinity(leftValue) && double.IsNegativeInfinity(rightValue)))
			{
				return true;
			}

			return (rightValue - leftValue) <= tolerance;
		}

		/// <summary>
		/// IsLesser - Determines whether leftValue is lesser than rightValue
		/// </summary>
		/// <param name="leftValue">Value on LHS of operator</param>
		/// <param name="rightValue">Value on RHS of operator</param>
		/// <param name="tolerance">Tolerance level for comparison</param>
		/// <returns>Return true if the given leftValue is lesser than rightValue. Return false otherwise</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsLesser(this double leftValue, double rightValue, double tolerance = 1e-10)
		{
			return (rightValue - leftValue) >= tolerance;
		}

		/// <summary>
		/// IsLesserOrEqual - Determines whether the leftValue is lesser or equal to rightValue
		/// </summary>
		/// <param name="leftValue">Value on LHS of operator</param>
		/// <param name="rightValue">Value on RHS of operator</param>
		/// <param name="tolerance">Tolerance level for comparison</param>
		/// <returns>Return true if the given leftValue is lesser than or equal to rightValue. Return false otherwise</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsLesserOrEqual(this double leftValue, double rightValue, double tolerance = 1e-10)
		{
			if ((double.IsPositiveInfinity(leftValue) && double.IsPositiveInfinity(rightValue)) ||
				(double.IsNegativeInfinity(leftValue) && double.IsNegativeInfinity(rightValue)))
			{
				return true;
			}

			return (leftValue - rightValue) <= tolerance;
		}

		/// <summary>
		/// Checks, if value is zero with specified tolerance.
		/// </summary>
		/// <param name="value">The value for check.</param>
		/// <param name="tolerance">The precision of check.</param>
		/// <returns>True, if value is zero, false otherwise.</returns>
		public static bool IsZero(this double value, double tolerance = 1e-9)
		{
			return Math.Abs(value) < tolerance;
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
