using System;

namespace CI.Mathematics
{
	/// <summary>
	/// Numerical iterations methods.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1407:ArithmeticExpressionsMustDeclarePrecedence", Justification = "Reviewed by HRO.")]
	public static class Iterationsa
	{
		private const double DerivationSmallChange = 1e-6;
		private static int maxIterSteps = 100;

		/// <summary>
		/// Sets the maximal number of iterations.
		/// </summary>
		public static int MaxIterSteps
		{
			set { maxIterSteps = value; }
		}

		#region Numerical differentiation

		/// <summary>
		/// Numerical differentiation, forward method.
		/// Produce an estimate of the derivative of a mathematical function.
		/// </summary>
		/// <typeparam name="T">The type of parameters needed for function.</typeparam>
		/// <param name="f">The function for evaluation.</param>
		/// <param name="param">The parameter needed for evaluation of specified function.</param>
		/// <param name="x">The position for required derivation.</param>
		/// <returns>The derivative of function in x.</returns>
		public static double DifferentiateForward<T>(Func<double, T, double> f, T param, double x)
		{
			return (f(x + DerivationSmallChange, param) - f(x, param)) / DerivationSmallChange;
		}

		/// <summary>
		/// Numerical differentiation, forward method.
		/// Produce an estimate of the derivative of a mathematical function.
		/// </summary>
		/// <typeparam name="T">The type of parameters needed for function.</typeparam>
		/// <param name="f">The function for evaluation.</param>
		/// <param name="param">The parameter needed for evaluation of specified function.</param>
		/// <param name="x">The position for required derivation.</param>
		/// <returns>The derivative of function in x.</returns>
		public static double DifferentiateCentral<T>(Func<double, T, double> f, T param, double x)
		{
			return (f(x + DerivationSmallChange, param) - f(x - DerivationSmallChange, param)) / 2.0 / DerivationSmallChange;
		}

		#endregion

		#region Iterations

		/// <summary>
		/// Finds iteratively the root of specfied function.
		/// Function must fulfil the condition f(init1)*f(init2) &lt; 0.
		/// </summary>
		/// <typeparam name="T">The type of parameters needed for function.</typeparam>
		/// <param name="f">The function for evaluation.</param>
		/// <param name="param">The parameter needed for evaluation of specified function.</param>
		/// <param name="a">The begin of the interval.</param>
		/// <param name="b">The end of the interval.</param>
		/// <param name="tolerance">The calculation tolerance.</param>
		/// <param name="value">The calculated value X, where the function has zero.</param>
		/// <returns>True if success, false otherwise.</returns>
		public static bool BisectionMethod<T>(Func<double, T, double> f, T param, double a, double b, double tolerance, out double value)
		{
			double x0 = a;
			double x1 = b;
			double y0 = f(x0, param);
			if (y0.IsZero(tolerance))
			{
				value = x0;
				return true;
			}

			double y1 = f(x1, param);
			if (y1.IsZero(tolerance))
			{
				value = x1;
				return true;
			}

			if ((y0 * y1) < 0)
			{
				double x, y;
				for (int i = maxIterSteps; i > 0; --i)
				{
					x = x0 / 2 + x1 / 2;
					y = f(x, param);
					if (y.IsZero(tolerance))
					{
						value = x;
						return true;
					}

					if ((y0 * y) < 0)
					{
						x1 = x;
						y1 = y;
					}
					else
					{
						x0 = x;
						y0 = y;
					}
				}
			}

			value = x0;
			return false;
		}

		/// <summary>
		/// Finds iteratively the root of specfied function.
		/// Function must fulfil the condition f(init1)*f(init2) &lt; 0.
		/// </summary>
		/// <typeparam name="T">The type of parameters needed for function.</typeparam>
		/// <param name="f">The function for evaluation.</param>
		/// <param name="param">The parameter needed for evaluation of specified function.</param>
		/// <param name="initialX">The initial x value.</param>
		/// <param name="tolerance">The calculation tolerance.</param>
		/// <param name="value">The calculated value X, where the function has zero.</param>
		/// <returns>True if success, false otherwise.</returns>
		public static bool NewtonMethod<T>(Func<double, T, double> f, T param, double initialX, double tolerance, out double value)
		{
			double x0 = initialX;
			double y0 = f(x0, param);
			if (y0.IsZero(tolerance))
			{
				value = x0;
				return true;
			}

			double x, y;
			for (int i = maxIterSteps; i > 0; --i)
			{
				x = x0 - y0 / DifferentiateCentral(f, param, x0);
				y = f(x, param);
				if (y.IsZero(tolerance))
				{
					value = x;
					return true;
				}

				x0 = x;
				y0 = y;
			}

			value = x0;
			return false;
		}

		/// <summary>
		/// Finds iteratively the root of specfied function.
		/// </summary>
		/// <typeparam name="T">The type of parameters needed for function.</typeparam>
		/// <param name="f">The function for evaluation.</param>
		/// <param name="param">The parameter needed for evaluation of specified function.</param>
		/// <param name="init1">The initial X value 1.</param>
		/// <param name="init2">The initial X value 2</param>
		/// <param name="tolerance">The calculation tolerance.</param>
		/// <param name="value">The calculated value X, where the function has zero.</param>
		/// <returns>True if success, false otherwise.</returns>
		public static bool SecantMethod<T>(Func<double, T, double> f, T param, double init1, double init2, double tolerance, out double value)
		{
			double x0 = init1;
			double x1 = init2;
			double y0 = f(x0, param);
			if (y0.IsZero(tolerance))
			{
				value = x0;
				return true;
			}

			double y1 = f(x1, param);
			if (y1.IsZero(tolerance))
			{
				value = x1;
				return true;
			}

			double x, y;

			for (int i = maxIterSteps; i > 0; --i)
			{
				x = y1 * x0 / (y1 - y0) + y0 * x1 / (y0 - y1);
				y = f(x, param);
				if (y.IsZero(tolerance))
				{
					value = x;
					return true;
				}

				x0 = x1;
				y0 = y1;
				x1 = x;
				y1 = y;
			}

			value = x0;
			return false;
		}

		#endregion

		//// TODO regula falsi, ...
	}
}
