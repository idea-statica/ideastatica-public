using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Material headed stud grade
	/// </summary>
	[OpenModelClass("CI.StructModel.Libraries.Material.MaterialHeadedStudGrade,CI.Material", "CI.StructModel.Libraries.Material.IMatHeadedStudGrade,CI.BasicTypes", typeof(Material))]
	[DataContract]
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class MaterialHeadedStudGrade : Material
	{
		#region Properties

		/// <summary>
		/// Ultimate tensile strength
		/// </summary>
#pragma warning disable IDE1006 // Naming Styles

		public double fub
#pragma warning restore IDE1006 // Naming Styles
		{
			get; set;
		}

		/// <summary>
		/// Yield strength
		/// </summary>
#pragma warning disable IDE1006 // Naming Styles

		public double fyb
#pragma warning restore IDE1006 // Naming Styles
		{
			get; set;
		}

		/// <summary>
		/// Elongation after fracture - for 20% elongation use value 0.2
		/// </summary>
		public double Elongation
		{
			get; set;
		}

		/// <summary>
		/// Design code
		/// </summary>
		public CountryCode Code
		{
			get; set;
		}


		/// <summary>
		/// Name fo the bolt grade in MPRL database
		/// </summary>
		[XmlIgnore]
		public Guid MprlElementID
		{
			get; set;
		}


		/// <summary>
		/// Unique ID of MPRL database table where the bolt grade was taken from.
		/// </summary>
		[XmlIgnore]
		public Guid MprlTableID
		{
			get; set;
		}

		#endregion Properties
	}
}