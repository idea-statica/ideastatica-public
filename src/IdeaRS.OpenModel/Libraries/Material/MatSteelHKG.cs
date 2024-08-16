using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Material steel HKG
	/// </summary>
	[OpenModelClass("CI.StructModel.Libraries.Material.HKG.MatSteelHKG,CI.Material", "CI.StructModel.Libraries.Material.IMaterial,CI.BasicTypes", typeof(MatSteel))]
	[DataContract]
	public class MatSteelHKG : MatSteel
	{
		/// <summary>
		/// Yield strength - Ys
		/// </summary>
		[DataMember]
		public double fy { get; set; }

		/// <summary>
		/// Ultimate strength - Yu
		/// </summary>
		[DataMember]
		public double fu { get; set; }


		/// <summary>
		/// Material strength for specific thickness of plate
		/// </summary>
		[DataMember]
		public MaterialStrengthProperty MaterialStrength { get; set; }

		/// <summary>
		/// Overstrength coefficient for fu
		/// </summary>
		[DataMember]
		public double GammaOVfu
		{
			get;
			set;
		}

		/// <summary>
		/// Overstrength coefficient for fy
		/// </summary>
		[DataMember]
		public double GammaOVfy
		{
			get;
			set;
		}
	}
}
