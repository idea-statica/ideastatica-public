using System.Reflection;

namespace IdeaRS.OpenModel.Result
{
	/// <summary>
	/// Point Result of NLA
	/// </summary>
	[Obfuscation(Feature = "renaming")]
	public class PointResultOfNLA : PointResultBase
	{
		/// <summary>
		/// Axial force
		/// </summary>
		public double AxialForce { get; set; }

		/// <summary>
		/// Axial stress
		/// </summary>
		public double AxialStress { get; set; }

		/// <summary>
		/// Axial strain
		/// </summary>
		public double AxialStrain { get; set; }
	}
}