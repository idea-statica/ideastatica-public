using System;
using System.Windows;

namespace CI.Mathematics
{
	/// <summary>
	/// Provides a mathematical tools.
	/// </summary>
	public class Tools
	{
		/// <summary>
		/// Interchanges two values.
		/// </summary>
		/// <typeparam name="T">The type of values.</typeparam>
		/// <param name="lhs">The first value.</param>
		/// <param name="rhs">The second value.</param>
		public static void Swap<T>(ref T lhs, ref T rhs)
		{
			T temp;
			temp = lhs;
			lhs = rhs;
			rhs = temp;
		}

		/// <summary>
		/// Returns the largest of three double-precision floating-point numbers.
		/// </summary>
		/// <param name="val1">The first of three double-precision floating-point numbers to compare.</param>
		/// <param name="val2">The second of three double-precision floating-point numbers to compare.</param>
		/// <param name="val3">The third of three double-precision floating-point numbers to compare.</param>
		/// <returns>Parameter val1, val2 or val3, whichever is larger. If val1, val2, val3 or all
		/// values are equal to <c>System.Double.NaN</c>, <c>System.Double.NaN</c> is returned.</returns>
		public static double Max(double val1, double val2, double val3)
		{
			return Math.Max(val1, Math.Max(val2, val3));
		}

		/// <summary>
		/// Returns the smaller of three 32-bit signed integers.
		/// </summary>
		/// <param name="val1">The first of three 32-bit signed integers to compare.</param>
		/// <param name="val2">The second of three 32-bit signed integers to compare.</param>
		/// <param name="val3">The third of three 32-bit signed integers to compare.</param>
		/// <returns>Parameter val1, val2 or val3, whichever is smaller.</returns>
		public static int Min(int val1, int val2, int val3)
		{
			return Math.Min(val1, Math.Min(val2, val3));
		}

		/// <summary>
		/// Returns the smaller of three double-precision floating-point numbers.
		/// </summary>
		/// <param name="val1">The first of three double-precision floating-point numbers to compare.</param>
		/// <param name="val2">The second of three double-precision floating-point numbers to compare.</param>
		/// <param name="val3">The third of three double-precision floating-point numbers to compare.</param>
		/// <returns>Parameter val1, val2 or val3, whichever is smaller. If val1, val2, val3 or all
		/// values are equal to <c>System.Double.NaN</c>, <c>System.Double.NaN</c> is returned.</returns>
		public static double Min(double val1, double val2, double val3)
		{
			return Math.Min(val1, Math.Min(val2, val3));
		}

		/// <summary>
		/// Returns the largest of two absolute values of double-precision floating-point numbers.
		/// </summary>
		/// <param name="val1">The first of two double-precision floating-point numbers to compare.</param>
		/// <param name="val2">The second of two double-precision floating-point numbers to compare.</param>
		/// <returns>Parameter val1 or val2, whichever absolute value is larger.</returns>
		public static double MaxAbs(double val1, double val2)
		{
			if (Math.Abs(val1) < Math.Abs(val2))
			{
				return val2;
			}
			else
			{
				return val1;
			}
		}

		/// <summary>
		/// Returns the smalles of two absolute values of double-precision floating-point numbers.
		/// </summary>
		/// <param name="val1">The first of two double-precision floating-point numbers to compare.</param>
		/// <param name="val2">The second of two double-precision floating-point numbers to compare.</param>
		/// <returns>Parameter val1 or val2, whichever absolute value is smaller.</returns>
		public static double MinAbs(double val1, double val2)
		{
			if (Math.Abs(val1) > Math.Abs(val2))
			{
				return val2;
			}
			else
			{
				return val1;
			}
		}

		/// <summary>
		/// Returns a value indicating the sign of a double-precision floating-point number.
		/// If number is zero, returns 1!
		/// </summary>
		/// <param name="value">A signed number.</param>
		/// <returns>A number that indicates the sign of value, as shown in the following table.
		/// Return value Meaning -1 value is less than zero. 1 value is greater or equal to zero.</returns>
		public static int Sign(double value)
		{
			return value < 0 ? -1 : 1;
		}

		/// <summary>
		/// Interpolates linearly between two <c>System.Windows.Point</c>.
		/// </summary>
		/// <param name="a">The first of two <c>System.Windows.Point</c>.</param>
		/// <param name="b">The second of two <c>System.Windows.Point</c>.</param>
		/// <param name="x">The coordinate x for which is searched y coordinate.</param>
		/// <param name="extrapolate">True, if extrapolation is required.</param>
		/// <returns>The y coordinate related to x coordinate.</returns>
		public static double Interpolate(Point a, Point b, double x, bool extrapolate)
		{
			if (!a.X.IsEqual(b.X))
			{
				if (!extrapolate)
				{
					if (a.X > b.X)
					{
						Swap(ref a, ref b);
					}

					if (x <= a.X)
					{
						return a.Y;
					}
					else if (x >= b.X)
					{
						return b.Y;
					}
				}

				return a.Y + ((b.Y - a.Y) / (b.X - a.X) * (x - a.X));
			}
			else
			{
				if (x <= a.X)
				{
					return a.Y;
				}
				else
				{
					return b.Y;
				}
			}
		}

		/// <summary>
		/// Interpolates linearly.
		/// </summary>
		/// <param name="xa">x position of first point.</param>
		/// <param name="ya">y value of first point.</param>
		/// <param name="xb">x position of second point.</param>
		/// <param name="yb">y value of second point.</param>
		/// <param name="x">The coordinate x for which is searched y coordinate.</param>
		/// <param name="extrapolate">The y coordinate related to x coordinate.</param>
		/// <returns></returns>
		public static double Interpolate(double xa, double ya, double xb, double yb, double x, bool extrapolate)
		{
			if (!xa.IsEqual(xb))
			{
				if (!extrapolate)
				{
					if (xa > xb)
					{
						Swap(ref xa, ref xb);
						Swap(ref ya, ref yb);
					}

					if (x <= xa)
					{
						return ya;
					}
					else if (x >= xb)
					{
						return yb;
					}
				}

				return ya + ((yb - ya) / (xb - xa) * (x - xa));
			}
			else
			{
				if (x <= xa)
				{
					return ya;
				}
				else
				{
					return yb;
				}
			}
		}

		/// <summary>
		/// Interpolates linearly between two <c>System.Windows.Point</c>.
		/// </summary>
		/// <param name="a">The first of two <c>System.Windows.Point</c>.</param>
		/// <param name="b">The second of two <c>System.Windows.Point</c>.</param>
		/// <param name="x">The coordinate x for which is searched y coordinate.</param>
		/// <param name="extrapolate">True, if extrapolation is required.</param>
		/// <returns>The y coordinate related to x coordinate.</returns>
		public static double InterpolateLog(Point a, Point b, double x, bool extrapolate)
		{
			if (!a.X.IsEqual(b.X))
			{
				if (!extrapolate)
				{
					if (a.X > b.X)
					{
						Swap(ref a, ref b);
					}

					if (x <= a.X)
					{
						return a.Y;
					}
					else if (x >= b.X)
					{
						return b.Y;
					}
				}

				return a.Y + ((b.Y - a.Y) / (Math.Log(b.X) - Math.Log(a.X)) * (Math.Log(x) - Math.Log(a.X)));
			}
			else
			{
				if (x <= a.X)
				{
					return a.Y;
				}
				else
				{
					return b.Y;
				}
			}
		}

		/// <summary>
		/// Returns the inverse hyperbolic sine of the specified angle (sinh^-1).
		/// </summary>
		/// <param name="value">An angle, measured in radians.</param>
		/// <returns>The inverse hyperbolic sine of value.</returns>
		public static double Asinh(double value)
		{
			return Math.Log(value + Math.Sqrt(value * value + 1));
		}

		/// <summary>
		/// Returns the inverse hyperbolic cosine of the specified angle (cosh^-1).
		/// </summary>
		/// <param name="value">An angle, measured in radians.</param>
		/// <returns>The inverse hyperbolic cosine of value.</returns>
		public static double Acosh(double value)
		{
			return Math.Log(value + Math.Sqrt(value * value - 1));
		}

		/// <summary>
		/// Returns the inverse hyperbolic tangent of the specified angle (tanh^-1).
		/// </summary>
		/// <param name="value">An angle, measured in radians.</param>
		/// <returns>The inverse hyperbolic tangent of value.</returns>
		public static double Atanh(double value)
		{
			return Math.Log((1 + value) / (1 - value)) / 2;
		}

		/// <summary>
		/// Returns bool value if the value is in interval.
		/// </summary>
		/// <param name="val1">The first value of interval.</param>
		/// <param name="val2">The second value of interval.</param>
		/// <param name="val">The tested value of interval.</param>
		/// <param name="precission">The preciision of testing.</param>
		/// <returns>true- is in interval, false- out of interval</returns>
		public static bool IsInInterval(double val1, double val2, double val, double precission = 1e-9)
		{
			if (val1 > val2)
			{
				Tools.Swap(ref val1, ref val2);
			}
			return (val1.IsLesserOrEqual(val, precission) && val2.IsGreaterOrEqual(val, precission));
		}
	}
}