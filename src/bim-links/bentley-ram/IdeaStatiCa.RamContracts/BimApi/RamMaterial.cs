using ISteelMaterial = RAMDATAACCESSLib.ISteelMaterial;
using IConcreteMaterial = RAMDATAACCESSLib.IConcreteMaterial;
using IOtherMaterial = RAMDATAACCESSLib.IOtherMaterial;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdeaStatiCa.BimApi;
using IdeaRS.OpenModel.Material;

namespace IdeaStatiCa.RamToIdea.BimApi
{
	public abstract class RamMaterial
	{
		/// <summary>
		/// Gravitation acceleration
		/// </summary>
		private const double g = 10.0;


		//TODO FillMaterialDataFromSteel

		//TODO FillMaterialDataFromConcrete

		//TODO FillMaterialDataFromOther


		protected void FillMaterialData(RamMaterialWrapper matRam, IdeaRS.OpenModel.Material.Material matIom)
		{
			matIom.E = matRam.E;
			
			//no shear modulus avaliable. Use typical equation.
			//matIom.G = matRstab.ShearModulus;
			
			matIom.Poisson = matRam.Poissons;
			// SpecificWeight is in N/m^3, UnitMass is kg/m^3
			matIom.UnitMass = matRam.Density;
			
			//ThermalExpansion Is not avliable in Ram. use typical value
			matIom.ThermalExpansion = 12e-3;
			matIom.SpecificHeat = 0.6;
			matIom.ThermalConductivity = 45;

			matIom.StateOfThermalConductivity = ThermalConductivityState.Code;
			matIom.StateOfThermalExpansion = ThermalExpansionState.Code;
			matIom.StateOfThermalSpecificHeat = ThermalSpecificHeatState.Code;
			matIom.StateOfThermalStressStrain = ThermalStressStrainState.Code;
		}
		//High level materials are set in RAM Modeller and 

	}
}
