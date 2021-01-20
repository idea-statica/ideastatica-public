using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Anchor custom data
	/// </summary>
	public class MatAnchorProperties
	{
		/// <summary>
		/// Tension linear stiffness.
		/// </summary>
		public double KT { get; set; }

		/// <summary>
		/// Nonlinear stiffness in tension after over loaded FLT
		/// </summary>
		public double KTN { get; set; }

		/// <summary>
		/// Limit force in tension
		/// </summary>
		public double FLT { get; set; }

		/// <summary>
		/// Limit deformation in tension for stop solution UCRIT
		/// </summary>
		public double Ult { get; set; }

		/// <summary>
		/// Shear linear stiffness.
		/// </summary>
		public double KS { get; set; }

		/// <summary>
		/// Nonlinear stiffness in shear after over loaded FLS
		/// </summary>
		public double KSN { get; set; }

		/// <summary>
		/// Limit force in shear.
		/// </summary>
		public double FLS { get; set; }

		/// <summary>
		/// Limit deformation in shear for stop solution UCRIT
		/// </summary>
		public double Uls { get; set; }
	}
}
