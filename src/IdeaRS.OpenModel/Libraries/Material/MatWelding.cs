using System.Xml.Serialization;

namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Welding material base class
	/// </summary>
	[XmlInclude(typeof(MatWeldingEc2))]
	[XmlInclude(typeof(MatWeldingAISC))]
	[XmlInclude(typeof(MatWeldingCISC))]
	[XmlInclude(typeof(MatWeldingAUS))]
	[XmlInclude(typeof(MatWeldingRUS))]
	[XmlInclude(typeof(MatWeldingCHN))]
	[XmlInclude(typeof(MatWeldingIS))]
	[XmlInclude(typeof(MatWeldingHKG))]
	public abstract class MatWelding : Material
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public MatWelding()
		{
			SetDefaultMatSteelProperties();
		}

		private void SetDefaultMatSteelProperties()
		{
			// Common material properties
			UnitMass = 7850;
			E = 2.1e11;
			G = 8.0769e10;
			Poisson = 0.3;
			SpecificHeat = 0.6;
			ThermalExpansion = 0.14e-3;
			ThermalConductivity = 45;			
		}
	}
}
