using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Material;
using IdeaStatiCa.BimApi;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal class MaterialImporter : AbstractImporter<IIdeaMaterial>
	{
		protected override OpenElementId ImportInternal(ImportContext ctx, IIdeaMaterial material)
		{
			MatSteelEc2 matS = new MatSteelEc2();
			matS.Name = "S275";
			matS.UnitMass = 7850.0;
			matS.E = 200e9;
			matS.Poisson = 0.2;
			matS.G = 83.333e9;
			matS.SpecificHeat = 0.6;
			matS.ThermalExpansion = 0.00001;
			matS.ThermalConductivity = 45;
			matS.fy = 235e6;
			matS.fu = 360e6;
			matS.fy40 = 215e6;
			matS.fu40 = 340e6;

			//Setting thermal characteristcs of material (in this case by the code)
			matS.StateOfThermalConductivity = ThermalConductivityState.Code;
			matS.StateOfThermalExpansion = ThermalExpansionState.Code;
			matS.StateOfThermalSpecificHeat = ThermalSpecificHeatState.Code;
			matS.StateOfThermalStressStrain = ThermalStressStrainState.Code;

			return matS;
		}
	}
}