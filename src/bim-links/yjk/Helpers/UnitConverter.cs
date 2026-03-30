using IdeaRS.OpenModel.CrossSection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yjk.Helpers
{
	internal static class UnitConverter
	{
		public static double MmToM(double mm)
		{
			return mm / 1000;
		}

		public static double DegToRad(double degree)
		{
			return degree * Math.PI / 180;
		}
	}
}
