namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Welding material for ECEN
	/// </summary>
	public class MatWeldingEc2 : MatWelding
	{
		/// <summary>
		/// Ultimate Strength
		/// </summary>
		public double fu { get; set; }

		/// <summary>
		/// BetaW value
		/// </summary>
		public double BetaW { get; set; }
	}
}
