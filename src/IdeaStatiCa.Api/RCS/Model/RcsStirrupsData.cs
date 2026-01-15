using IdeaRS.OpenModel.Geometry2D;

namespace IdeaStatiCa.Api.RCS.Model
{
	/// <summary>
	/// Stirrup input
	/// </summary>
	public class RcsStirrupsData
	{
		/// <summary>
		/// Stirrup diameter in meters
		/// </summary>
		public double Diameter { get; set; }

		/// <summary>
		/// Reference to the reinforcement material (must exist in project)
		/// </summary>
		public string MaterialName { get; set; }

		/// <summary>
		/// Diameter of mandrel for bending
		/// </summary>
		public double DiameterOfMandrel { get; set; }

		/// <summary>
		/// Is the stirrup closed
		/// </summary>
		public bool IsClosed { get; set; }

		/// <summary>
		/// Include in shear check
		/// </summary>
		public bool ShearCheck { get; set; }

		/// <summary>
		/// Include in torsion check
		/// </summary>
		public bool TorsionCheck { get; set; }

		/// <summary>
		/// Distance between stirrups
		/// </summary>
		public double Distance { get; set; }

		/// <summary>
		/// Geometry of the stirrup
		/// </summary>
		public PolyLine2D Geometry { get; set; }
	}
}