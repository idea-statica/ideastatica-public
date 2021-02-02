using System;
namespace CI.Mathematics
{
	/// <summary>
	/// Provides basic conversion functions
	/// </summary>
	public static class Conversions
	{
		/// <summary>
		/// conversion from system to MPa
		/// </summary>
		/// <param name="value">value for conversion</param>
		/// <returns>converted value</returns>
		public static double SystemToMPa(double value)
		{
			value /= 1e6;
			return value;
		}

		/// <summary>
		/// conversion from MPa to system
		/// </summary>
		/// <param name="value">value for conversion</param>
		/// <returns>converted value</returns>
		public static double MPaToSystem(double value)
		{
			value *= 1e6;
			return value;
		}

		/// <summary>
		/// conversion from system to GPa
		/// </summary>
		/// <param name="value">value for conversion</param>
		/// <returns>converted value</returns>
		public static double SystemToGPa(double value)
		{
			value /= 1e9;
			return value;
		}

		/// <summary>
		/// conversion from GPa to system
		/// </summary>
		/// <param name="value">value for conversion</param>
		/// <returns>converted value</returns>
		public static double GPaToSystem(double value)
		{
			value *= 1e9;
			return value;
		}

		/// <summary>
		/// conversion from system to Promile
		/// </summary>
		/// <param name="value">value for conversion</param>
		/// <returns>converted value</returns>
		public static double SystemToPromile(double value)
		{
			return value * 1e3;
		}

		/// <summary>
		/// conversion from Promile to system
		/// </summary>
		/// <param name="value">value for conversion</param>
		/// <returns>converted value</returns>
		public static double PromileToSystem(double value)
		{
			return value / 1e3;
		}

		/// <summary>
		/// Conversion unit weight to weight density
		/// </summary>
		/// <param name="value">Unit weigth</param>
		/// <returns>Weight density</returns>
		public static double UnitWeightToWeightDensity(double value)
		{
			return value * MathConstants.gn;
		}

		/// <summary>
		/// Conversion weight density to unit weight
		/// </summary>
		/// <param name="value">Weigth density</param>
		/// <returns>Unit weight</returns>
		public static double WeightDensityToUnitWeight(double value)
		{
			return value / MathConstants.gn;
		}

		/// <summary>
		/// conversion from system to days
		/// </summary>
		/// <param name="value">value for conversion</param>
		/// <returns>converted value</returns>
		public static double SystemToDays(double value)
		{
			return value / (24 * 60 * 60);
		}

		/// <summary>
		/// conversion from days to system
		/// </summary>
		/// <param name="value">value for conversion</param>
		/// <returns>converted value</returns>
		public static double DaysToSystem(double value)
		{
			return value * 24 * 60 * 60;
		}

		/// <summary>
		/// conversion from system to minutes
		/// </summary>
		/// <param name="value">value for conversion</param>
		/// <returns>converted value</returns>
		public static double SystemToMinutes(double value)
		{
			return value / (60);
		}

		/// <summary>
		/// conversion from minutes to system
		/// </summary>
		/// <param name="value">value for conversion</param>
		/// <returns>converted value</returns>
		public static double MinutesToSystem(double value)
		{
			return value * 60;
		}

		/// <summary>
		/// conversion from system to years
		/// </summary>
		/// <param name="value">value for conversion</param>
		/// <returns>converted value</returns>
		public static double SystemToYears(double value)
		{
			return value / (24 * 60 * 60 * 365);
		}

		/// <summary>
		/// conversion from years to system
		/// </summary>
		/// <param name="value">value for conversion</param>
		/// <returns>converted value</returns>
		public static double YearsToSystem(double value)
		{
			return value * 24 * 60 * 60 * 365;
		}

		/// <summary>
		/// conversion to logarithmic value
		/// </summary>
		/// <param name="value">value</param>
		/// <param name="min">minimal value</param>
		/// <returns>converted value</returns>
		public static double ToLogAxis(double value, double min)
		{
			return Math.Log(1 - min + value);
		}

		/// <summary>
		/// conversion from radians to degrees
		/// </summary>
		/// <param name="value">value for conversion</param>
		/// <returns>converted value</returns>
		public static double RadiansToDegrees(double value)
		{
			return value * 180 / Math.PI;
		}

		/// <summary>
		/// conversion from degrees to radians
		/// </summary>
		/// <param name="value">value for conversion</param>
		/// <returns>converted value</returns>
		public static double DegreesToRadians(double value)
		{
			return value * Math.PI / 180;
		}

		/// <summary>
		/// conversion from centigrade to system
		/// </summary>
		/// <param name="value">value for conversion</param>
		/// <returns>converted value</returns>
		public static double CentigradeToSystem(double value)
		{
			return value + 273.15;
		}

		/// <summary>
		/// conversion from system to centigrade
		/// </summary>
		/// <param name="value">value for conversion</param>
		/// <returns>converted value</returns>
		public static double SystemToCentigrade(double value)
		{
			return value - 273.15;
		}

		/// <summary>
		/// Converts value of <paramref name="angle"/> to angle -0.5PI, 0.5PI
		/// </summary>
		/// <param name="angle"></param>
		public static double AngleToPlusMinus05PI(double angle)
		{
			if(angle < -MathConstants.PI_2)
			{
				int n = (int)(angle / (-MathConstants.PI_2));
				return Math.Abs(angle + (n * MathConstants.PI_2));
			}

			if(angle > MathConstants.PI_2)
			{
				int n = (int)(angle / MathConstants.PI_2);
				return -1 * (angle - (n * MathConstants.PI_2));
			}

			return angle;
		}

		/// <summary>
		/// conversion from Pascal to psi
		/// </summary>
		/// <param name="value">value for conversion</param>
		/// <returns>converted value</returns>
		public static double PascalToPsi(double value)
		{
			return value / 6894.75729;
		}

		/// <summary>
		/// conversion from psi to Pascal
		/// </summary>
		/// <param name="value">value for conversion</param>
		/// <returns>converted value</returns>
		public static double PsiToPascal(double value)
		{
			return value * 6894.75729;
		}

		/// <summary>
		/// conversion from meter to inch
		/// </summary>
		/// <param name="value">value for conversion</param>
		/// <returns>converted value</returns>
		public static double MeterToInch(double value)
		{
			return value / 0.0254;
		}

		/// <summary>
		/// conversion from inch to meter
		/// </summary>
		/// <param name="value">value for conversion</param>
		/// <returns>converted value</returns>
		public static double InchToMeter(double value)
		{
			return value * 0.0254;
		}
	}
}
