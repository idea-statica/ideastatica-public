
namespace IdeaRS.OpenModel.Material
{
	public class MatWeldingHKG : MatWelding
	{
		/// <summary>
		/// Nominal Tensile Strength
		/// </summary>
		public double Ue { get; set; }

		public bool EcenElectrode { get; set; }
	}
}
