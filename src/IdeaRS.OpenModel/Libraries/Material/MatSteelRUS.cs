using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace IdeaRS.OpenModel.Material
{
    /// <summary>
	/// Material steel RUS
	/// </summary>
	[OpenModelClass("CI.StructModel.Libraries.Material.RUS.MatSteelRUS,CI.Material", "CI.StructModel.Libraries.Material.IMaterial,CI.BasicTypes", typeof(MatSteel))]
	[DataContract]
	public class MatSteelRUS : MatSteel
	{
		/// <summary>
		/// Yield strength
		/// </summary>
		[DataMember]
		public double fy { get; set; }

		/// <summary>
		/// Ultimate strength
		/// </summary>
		[DataMember]
		public double fu { get; set; }

		/// <summary>
		/// Safety coefficient
		/// </summary>
		[DataMember]
		public double GammaM { get; set; }

		///// <summary>
		///// Material strength for specific thickness of plate
		///// </summary>
		//public MaterialStrengthProperty MaterialStrength { get; set; }

		///// <summary>
		///// Overstrength coefficient for fu
		///// </summary>
		//public double GammaOVfu
		//{
		//	get;
		//	set;
		//}

		///// <summary>
		///// Overstrength coefficient for fy
		///// </summary>
		//public double GammaOVfy
		//{
		//	get;
		//	set;
		//}
	}
}
