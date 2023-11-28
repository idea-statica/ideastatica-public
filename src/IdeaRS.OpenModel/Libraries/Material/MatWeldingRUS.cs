namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Welding material for russian code
	/// </summary>
	public class MatWeldingRUS : MatWelding
	{
		/// <summary>
		/// Ultimate Strength
		/// </summary>
		public double Rwun { get; set; }

		/// <summary>
		/// Welding type
		/// </summary>
		public WeldingTypeSNIP WeldingType { get; set; }

		/// <summary>
		/// Flat position
		/// </summary>
		public bool FlatPosition { get; set; }
	}
}
