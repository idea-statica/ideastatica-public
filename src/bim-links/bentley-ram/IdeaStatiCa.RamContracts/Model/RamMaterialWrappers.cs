using RAMDATAACCESSLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaStatiCa.RamToIdea
{
	public class RamMaterialWrapper
	{
		public EMATERIALTYPES Type;

		public int Id;
		public double E;
		public double Poissons;
		public double Density;

		public RamMaterialWrapper(int id, double youngsModulus, double poisonsRatio, double density)
		{
			Id = id;
			E = youngsModulus;
			Poissons = poisonsRatio;
			Density = density;
		}
	}

	public class RamSteelMaterialWrapper : RamMaterialWrapper
	{
		public double Fy;
		public double Fu;
		public bool Composite = false;

		public RamSteelMaterialWrapper(int id, double youngsModulus, double poisonsRatio, double density, double fy, double fu, bool composite) : base(id, youngsModulus, poisonsRatio, density)
		{
			Fy = fy;
			Fu = fu;
			Composite = composite;
			Type = EMATERIALTYPES.ESteelMat;
		}
	}


	//public class RamConcreteWrapper
	//{
	//	
	//}

}
