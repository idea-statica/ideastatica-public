using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.Material
{

	/// <summary>
	/// Material steel AISC
	/// </summary>
	[OpenModelClass("CI.StructModel.Libraries.Material.AUS.MatSteelAUS,CI.Material", "CI.StructModel.Libraries.Material.IMaterial,CI.BasicTypes", typeof(MatSteel))]
	[DataContract]
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class MatSteelAUS : MatSteel
	{
		/// <summary>
		/// Default yield strength for nominal thickness of the element - f<sub>y</sub>
		/// </summary>
		public double fy { get; set; }

		/// <summary>
		/// Default ultimate strength  for nominal thickness of the element - f<sub>u</sub>
		/// </summary>
		public double fu { get; set; }

		/// <summary>
		/// Material strength for specific thickness of plate
		/// </summary>
		public MaterialStrengthProperty MaterialStrength { get; set; }

		/// <summary>
		/// Overstrength coefficient for fu
		/// </summary>
		public double PhiOMFu
		{
			get;
			set;
		}

		/// <summary>
		/// Overstrength coefficient for fy
		/// </summary>
		public double PhiOMFy
		{
			get;
			set;
		}

	}
}