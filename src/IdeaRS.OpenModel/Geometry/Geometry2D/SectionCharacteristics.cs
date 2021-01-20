using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IdeaRS.OpenModel.Geometry2D
{
	/// <summary>
	/// Section characteristics
	/// </summary>
	public class SectionCharacteristics
	{
		/// <summary>
		/// Gets or sets the area.
		/// </summary>
		public double A { get; set; }

		/// <summary>
		/// Gets or sets the first moment of area related to the Y axis.
		/// </summary>
		public double Sy { get; set; }

		/// <summary>
		/// Gets or sets the first moment of area related to the Z axis.
		/// </summary>
		public double Sz { get; set; }

		/// <summary>
		/// Gets or sets the second moment of area related to the Y axis.
		/// </summary>
		public double Iy { get; set; }

		/// <summary>
		/// Gets or sets the second moment of area related to the Z axis.
		/// </summary>
		public double Iz { get; set; }

		/// <summary>
		/// Gets or sets the product moment of area.
		/// </summary>
		public double Dyz { get; set; }

		/// <summary>
		/// Gets the centre of gravity related to the axis Y.
		/// </summary>
		public double Cgy { get; set; }

		/// <summary>
		/// Gets the centre of gravity related to the axis Z.
		/// </summary>
		public double Cgz { get; set; }

		/// <summary>
		/// Gets the radius of gyration related to the axis Y.
		/// Also known as iy.
		/// </summary>
		public double Rgy { get; set; }

		/// <summary>
		/// Gets the radius of gyration related to the axis Z.
		/// Also known as iz.
		/// </summary>
		public double Rgz { get; set; }

		/// <summary>
		/// Gets or sets the responding E modulus.
		/// </summary>
		public double E { get; set; }

		/// <summary>
		/// Gets or sets the Elastic section modulus related to axis Y.
		/// </summary>
		public double Wely { get; set; }

		/// <summary>
		/// Gets or sets the Elastic section modulus related to axis Z.
		/// </summary>
		public double Welz { get; set; }

		/// <summary>
		/// Gets or sets the Plastic section modulus related to axis Y.
		/// </summary>
		public double Wply { get; set; }

		/// <summary>
		/// Gets or sets the Plastic section modulus related to axis Z.
		/// </summary>
		public double Wplz { get; set; }

		/// <summary>
		/// Gets or sets the Warping constant.
		/// </summary>
		public double Iw { get; set; }

		/// <summary>
		/// Gets or sets the Torsional constant.
		/// </summary>
		public double It { get; set; }

		/// <summary>
		/// Gets or sets the Shear center coordinate Y related to the center of gravity.
		/// </summary>
		public double Y0 { get; set; }

		/// <summary>
		/// Gets or sets the Shear center coordinate Z related to the center of gravity.
		/// </summary>
		public double Z0 { get; set; }

		/// <summary>
		/// Shear area
		/// </summary>
		public double Ay { get; set; }

		/// <summary>
		/// Shear area
		/// </summary>
		public double Az { get; set; }
	}
}
