using IdeaRS.OpenModel.Libraries.Material;

namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Welding material for chinese code
	/// </summary>
	public class MatWeldingCHN : MatWelding
	{
		public double Ffw { get; set; }		
		public double fuw { get; set; }
		public WeldLoadTypeCHN Loading { get; set; }
	}
}
