using IdeaRS.OpenModel.Material;
using IdeaStatiCa.BimApi;

namespace IdeaRstabPlugin.BimApi
{
	/// <summary>
	/// Abstract class for RSTAB implementations of <see cref="IIdeaMaterial"/>.
	/// </summary>
	internal abstract class RstabMaterial
	{
		/// <summary>
		/// Gravitation acceleration
		/// </summary>
		private const double g = 10.0;

		protected void FillMaterialData(Dlubal.RSTAB8.Material matRstab, IdeaRS.OpenModel.Material.Material matIom)
		{
			matIom.E = matRstab.ElasticityModulus;
			matIom.G = matRstab.ShearModulus;
			matIom.Poisson = matRstab.PoissonRatio;
			// SpecificWeight is in N/m^3, UnitMass is kg/m^3
			matIom.UnitMass = matRstab.SpecificWeight / g;
			matIom.ThermalExpansion = matRstab.ThermalExpansion;
			matIom.SpecificHeat = 0.6;
			matIom.ThermalConductivity = 45;

			matIom.StateOfThermalConductivity = ThermalConductivityState.Code;
			matIom.StateOfThermalExpansion = ThermalExpansionState.Code;
			matIom.StateOfThermalSpecificHeat = ThermalSpecificHeatState.Code;
			matIom.StateOfThermalStressStrain = ThermalStressStrainState.Code;
		}
	}
}