using System;
using System.Collections.Generic;
using System.Text;

namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Welding material AISC
	/// </summary>
	public class MatWeldingAISC : MatWelding
	{
		/// <summary>
		/// Nominal Tensile Strength
		/// </summary>
		public double FEXX { get; set; }
	}
}
