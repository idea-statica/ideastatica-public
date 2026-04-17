namespace NorsokChecker.Models
{
	/// <summary>
	/// Geometric properties of a tubular (CHS) cross-section.
	/// All dimensions in mm, areas in mm², moduli in mm³, inertia in mm⁴.
	/// </summary>
	public class TubularGeometry
	{
		/// <summary>Outside diameter [mm]</summary>
		public double D { get; set; }

		/// <summary>Wall thickness [mm]</summary>
		public double t { get; set; }

		/// <summary>Cross-sectional area [mm²]: π/4·(D²-(D-2t)²)</summary>
		public double A { get; set; }

		/// <summary>Elastic section modulus [mm³]: π/32·(D⁴-(D-2t)⁴)/D</summary>
		public double W { get; set; }

		/// <summary>Plastic section modulus [mm³]: 1/6·(D³-(D-2t)³)</summary>
		public double Z { get; set; }

		/// <summary>Moment of inertia [mm⁴]: π/64·(D⁴-(D-2t)⁴)</summary>
		public double I { get; set; }

		/// <summary>Polar moment of inertia [mm⁴]: π/32·(D⁴-(D-2t)⁴)</summary>
		public double Ip { get; set; }

		/// <summary>Radius of gyration [mm]: √(I/A)</summary>
		public double i { get; set; }

		/// <summary>Inner diameter [mm]: D-2t</summary>
		public double Di { get; set; }

		/// <summary>D/t ratio (must be &lt; 120 per §6.3.1)</summary>
		public double DtRatio { get; set; }

		/// <summary>Z/W ratio (shape factor for tubulars)</summary>
		public double ZWRatio { get; set; }
	}
}
