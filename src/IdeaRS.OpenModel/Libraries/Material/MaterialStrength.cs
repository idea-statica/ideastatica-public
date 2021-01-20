using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Strength of material with specific thickness range
	/// </summary>
	[OpenModelClass("IdeaRS.MprlModel.Material.MaterialStrength,CI.BasicTypes")]
	public class MaterialStrength : OpenObject
	{
		/// <summary>
		/// higher value of the thickness range
		/// </summary>
		public double MaxThickness { get; set; }

		/// <summary>
		/// Yield strength
		/// </summary>
		public double Fy { get; set; }

		/// <summary>
		/// Tension strength
		/// </summary>
		public double Fu { get; set; }
	}
}
