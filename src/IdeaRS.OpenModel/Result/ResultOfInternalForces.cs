using System.Reflection;

namespace IdeaRS.OpenModel.Result
{
	/// <summary>
	/// Result of internal forces in the one position
	/// </summary>
	/// <remarks>Convention of internal forces on 1D members <br/>
	/// <img src="Images\InternalForcesConvention.png" /> <br/>
	/// Internal forces on 1D members perform following actions:<br/>
	/// •	positive bending moment My causes tension in cross-section fibers with negative z-coordinate<br/>
	/// •	positive bending moment Mz causes tension in fibers with negative y-coordinate<br/>
	/// •	positive torsional moment Mx acts about x-axis of 1D member<br/>
	/// •	positive axial force N acts in direction of x-axis of member and causes tension in cross-section fibers<br/>
	/// •	positive shear force Qz acts in direction of z-axis of cross-section<br/>
	/// •	positive shear force Qy acts in direction of y-axis of cross-section<br/>
	/// </remarks>
	[Obfuscation(Feature = "renaming")]
	public class ResultOfInternalForces : SectionResultBase
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ResultOfInternalForces()
		{
			N = Qy = Qz = Mx = My = Mz = 0.0;
		}

		///// <summary>
		///// Position
		///// </summary>
		//public double Position { get; set; }

		/// <summary>
		/// Normal force
		/// </summary>
		public double N { get; set; }

		/// <summary>
		/// Shear force in y dirrection
		/// </summary>
		public double Qy { get; set; }

		/// <summary>
		/// Shear force in z dirrection
		/// </summary>
		public double Qz { get; set; }

		/// <summary>
		/// Bending moment around x-axis
		/// </summary>
		public double Mx { get; set; }

		/// <summary>
		/// Bending moment around y-axis
		/// </summary>
		public double My { get; set; }

		/// <summary>
		/// Bending moment around z-axis
		/// </summary>
		public double Mz { get; set; }
	}
}