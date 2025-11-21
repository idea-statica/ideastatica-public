using RAMDATAACCESSLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaStatiCa.RamToIdea
{
	internal class RamMaterialWrapper
	{
		internal EMATERIALTYPES Type;

		internal int Id;
		internal double E;
		internal double Poissons;
		internal double Density;

		public RamMaterialWrapper(int id, double youngsModulus, double poisonsRatio, double density)
		{
			Id = id;
			E = youngsModulus;
			Poissons = poisonsRatio;
			Density = density;
		}
	}

	internal class RamSteelMaterialWrapper : RamMaterialWrapper
	{
		internal double Fy;
		internal double Fu;
		internal bool Composite = false;

		public RamSteelMaterialWrapper(int id, double youngsModulus, double poisonsRatio, double density, double fy, double fu, bool composite) : base(id, youngsModulus, poisonsRatio, density)
		{
			Fy = fy;
			Fu = fu;
			Composite = composite;
			Type = EMATERIALTYPES.ESteelMat;
		}
	}


	//internal class RamConcreteWrapper
	//{
	//	
	//}

}
