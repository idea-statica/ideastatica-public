using System;
using System.Collections.Generic;
using System.Text;

namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Welding material Ec2
	/// </summary>
	public class MatWeldingEc2 : MatWelding
	{
		/// <summary>
		/// Nominal Tensile Strength
		/// </summary>
		public double fu { get; set; }

		/// <summary>
		/// BetaW value
		/// </summary>
		public double BetaW { get; set; }
	}
}
