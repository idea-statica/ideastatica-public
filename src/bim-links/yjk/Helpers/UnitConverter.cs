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
		public static double CrossSectionDim(double dimension)
		{
			//mm to m
			return dimension / 1000;
		}
	}
}
