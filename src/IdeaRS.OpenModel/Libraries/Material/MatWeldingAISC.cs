namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Welding material for AISC
	/// </summary>
	public class MatWeldingAISC : MatWelding
	{
		/// <summary>
		/// Ultimate Strength
		/// </summary>
		public double Fexx { get; set; }
	}
}
