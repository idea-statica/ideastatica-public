using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Material steel HKG
	/// </summary>
	[OpenModelClass("CI.StructModel.Libraries.Material.HKG.MatSteelHKG,CI.Material", "CI.StructModel.Libraries.Material.IMaterial,CI.BasicTypes", typeof(MatSteel))]
	public class MatSteelHKG : MatSteel
	{
		/// <summary>
		/// Yield strength - Ys
		/// </summary>
		public double fy { get; set; }

		/// <summary>
		/// Ultimate strength - Yu
		/// </summary>
		public double fu { get; set; }


		/// <summary>
		/// Material strength for specific thickness of plate
		/// </summary>
		public MaterialStrengthProperty MaterialStrength { get; set; }

		/// <summary>
		/// Overstrength coefficient for fu
		/// </summary>
		public double GammaOVfu
		{
			get;
			set;
		}

		/// <summary>
		/// Overstrength coefficient for fy
		/// </summary>
		public double GammaOVfy
		{
			get;
			set;
		}
	}
}
