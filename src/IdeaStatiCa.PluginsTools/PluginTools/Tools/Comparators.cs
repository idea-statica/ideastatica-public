using System;

namespace CI.Mathematics
{
	/// <summary>
	/// Provides basic comparators. Pos - maybe some of them might be extensions of double
	/// </summary>
	public class Comparators
	{
		/// <summary>
		/// Check, if value is in the range (a, b).
		/// </summary>
		/// <param name="a">The first border of a interval.</param>
		/// <param name="b">The second border of a interval.</param>
		/// <param name="x">The value x</param>
		/// <param name="tolerance">The value of tolerance</param>
		/// <returns>True, if value is in the range (a, b), otherwise false.</returns>
		public static bool InIntervalNo(double a, double b, double x, double tolerance = 0)
		{
			// return (a < x && x < b) || (b < x && x < a);
			return (a.IsLesser(x, tolerance) && x.IsLesser(b, tolerance)) || (b.IsLesser(x, tolerance) && x.IsLesser(a, tolerance));
		}

		/// <summary>
		/// Check, if value is in the range &lt;a, b), where a is lesser than b, or in the range (a, b&gt;, where b is lesser than a.
		/// </summary>
		/// <param name="a">The first border of a interval.</param>
		/// <param name="b">The second border of a interval.</param>
		/// <param name="x">The value x</param>
		/// <param name="tolerance">The value of tolerance</param>
		/// <returns>True, if value is in the range &lt;a, b), otherwise false.</returns>
		public static bool InIntervalDown(double a, double b, double x, double tolerance = 0)
		{
			// return (a <= x && x < b) || (b <= x && x < a);
			return (a.IsLesserOrEqual(x, tolerance) && x.IsLesser(b, tolerance)) || (b.IsLesserOrEqual(x, tolerance) && x.IsLesser(a, tolerance));
		}

		/// <summary>
		/// Check, if value is in the range (a, b&gt;, where a is lesser than b, or in the range &lt;a, b), where b is lesser than a.
		/// </summary>
		/// <param name="a">The first border of a interval.</param>
		/// <param name="b">The second border of a interval.</param>
		/// <param name="x">The value x</param>
		/// <param name="tolerance">The value of tolerance</param>
		/// <returns>True, if value is in the range (a, b&gt;, otherwise false.</returns>
		public static bool InIntervalUp(double a, double b, double x, double tolerance = 0)
		{
			// return (a < x && x <= b) || (b < x && x <= a);
			return (a.IsLesser(x, tolerance) && x.IsLesserOrEqual(b, tolerance)) || (b.IsLesser(x, tolerance) && x.IsLesserOrEqual(a, tolerance));
		}

		/// <summary>
		/// Check, if value is in the range &lt;a, b&gt;.
		/// </summary>
		/// <param name="a">The first border of a interval.</param>
		/// <param name="b">The second border of a interval.</param>
		/// <param name="x">The value x</param>
		/// <param name="tolerance">The value of tolerance</param>
		/// <returns>True, if value is in the range &lt;a, b&gt;, otherwise false.</returns>
		public static bool InIntervalBoth(double a, double b, double x, double tolerance = 0)
		{
			// return (a <= x && x <= b) || (b <= x && x <= a);
			return (a.IsLesserOrEqual(x, tolerance) && x.IsLesserOrEqual(b, tolerance)) || (b.IsLesserOrEqual(x, tolerance) && x.IsLesserOrEqual(a, tolerance));
		}

		/// <summary>
		/// Compares two <c>System.Double</c>, if absolute approximation error is in the absolute tolerance
		/// and relative approximation error is in the relative tolerance.
		/// </summary>
		/// <param name="origin">The original value.</param>
		/// <param name="approx">The aproximated (numerical calculated) value.</param>
		/// <param name="absTol">The absolute tolerance.</param>
		/// <param name="relTol">The relative tolerance.</param>
		/// <returns>True, if approximation error is lesser or equal than tolerance, false otherwise.</returns>
		public static bool CheckError(double origin, double approx, double absTol, double relTol)
		{
			double d = origin - approx;
			if (Math.Abs(d) <= absTol || d.IsZero(relTol))
			{
				return true;
			}

			if (!origin.IsZero(relTol))
			{
				d /= origin;
			}

			return Math.Abs(d) < relTol;
		}
	}
}
