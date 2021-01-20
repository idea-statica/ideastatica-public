using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Material concrete IND
	/// </summary>
	[OpenModelClass("CI.StructModel.Libraries.Material.IND.MatConcreteIND,CI.Material", "CI.StructModel.Libraries.Material.IMaterial,CI.BasicTypes", typeof(MatConcrete))]
	public class MatConcreteIND : MatConcrete
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public MatConcreteIND()
		{

		}

		/// <summary>
		/// Compressive strength of concrete
		/// </summary>
		public double Fck { get; set; }
	}
}