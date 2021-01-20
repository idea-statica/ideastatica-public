using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Material concrete HKG
	/// </summary>
	[OpenModelClass("CI.StructModel.Libraries.Material.HKG.MatConcreteHKG,CI.Material", "CI.StructModel.Libraries.Material.IMaterial,CI.BasicTypes", typeof(MatConcrete))]
	public class MatConcreteHKG : MatConcrete
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public MatConcreteHKG()
		{

		}

		/// <summary>
		/// Compressive strength of concrete
		/// </summary>
		public double Fcu { get; set; }

		/// <summary>
		/// Ultimate compressive strain in the concrete
		/// </summary>
		public double Epscu2 { get; set; }
	}
}