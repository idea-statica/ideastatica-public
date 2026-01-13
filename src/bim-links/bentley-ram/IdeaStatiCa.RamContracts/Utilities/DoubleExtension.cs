using System;
using System.Runtime.CompilerServices;

namespace IdeaStatiCa.RamToIdea.Utilities
{
	public static class DoubleExtension
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double InchesToMeters(this double val)
		{
			return val * 0.0254;
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
	}
}