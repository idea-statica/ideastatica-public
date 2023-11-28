
namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Welding material for hong kongese code
	/// </summary>
	public class MatWeldingHKG : MatWelding
	{
		/// <summary>
		/// Ultimate Strength
		/// </summary>
		public double Ue { get; set; }

		public bool EcenElectrode { get; set; }
	}
}
