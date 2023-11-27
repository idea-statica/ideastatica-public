
namespace IdeaRS.OpenModel.Material
{
	public class MatWeldingRUS : MatWelding
	{
		/// <summary>
		/// Nominal Tensile Strength
		/// </summary>
		public double Rwun { get; set; }

		/// <summary>
		/// Welding type
		/// </summary>
		public WeldingTypeSNIP WeldingType { get; set; }
		
		public bool FlatPosition { get; set; }
	}
}
