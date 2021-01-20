using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Material concrete AUS
	/// </summary>
	[OpenModelClass("CI.StructModel.Libraries.Material.AUS.MatConcreteAUS,CI.Material", "CI.StructModel.Libraries.Material.IMaterial,CI.BasicTypes", typeof(MatConcrete))]
	public class MatConcreteAUS : MatConcrete
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public MatConcreteAUS()
		{

		}

		/// <summary>
		/// Compressive strength of concrete
		/// </summary>
		public double Fcc { get; set; }
	}
}