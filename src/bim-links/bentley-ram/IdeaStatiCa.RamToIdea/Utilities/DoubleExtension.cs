using System;
using System.Runtime.CompilerServices;

namespace IdeaStatiCa.RamToIdea.Utilities
{
	internal static class DoubleExtension
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
		public static double DegreesToRadians(this double val)
		{
			return val / 180.0 * Math.PI;
		}
	}
}