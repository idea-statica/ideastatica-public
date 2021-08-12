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
	}
}