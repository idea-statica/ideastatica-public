using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Bolt assembly
	/// </summary>
	[OpenModelClass("CI.StructModel.Libraries.Material.BoltAssembly,CI.Material", "CI.StructModel.Libraries.Material.BoltAssembly,CI.Material", typeof(BoltAssembly))]
	[DataContract]
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class BoltAssembly : OpenElementId
	{
		/// <summary>
		/// Name of bolt assembly
		/// </summary>
		public string Name { get; set; }

		#region Bolt

		/// <summary>
		/// Bolt grade
		/// </summary>
		public ReferenceElement BoltGrade { get; set; }

		/// <summary>
		/// Bolt diameter
		/// </summary>
		public double Diameter
		{
			get;
			set;
		}

		/// <summary>
		/// Size of bore Hole
		/// </summary>
		public double Borehole
		{
			get;
			set;
		}

		/// <summary>
		/// Diameter of the head
		/// </summary>
		public double HeadDiameter
		{
			get;
			set;
		}

		/// <summary>
		/// Second diameter of the head
		/// </summary>
		public double DiagonalHeadDiameter
		{
			get;
			set;
		}

		/// <summary>
		/// Thickness of head
		/// </summary>
		public double HeadHeight
		{
			get;
			set;
		}

		/// <summary>
		/// Gross cross-section area
		/// </summary>
		public double GrossArea
		{
			get;
			set;
		}

		/// <summary>
		/// Tensile stress area
		/// </summary>
		public double TensileStressArea
		{
			get;
			set;
		}
		#endregion

		#region Nut
		/// <summary>
		/// Thickness of Nut
		/// </summary>
		public double NutThickness
		{
			get;
			set;
		}

		#endregion
		#region Washer
		/// <summary>
		/// Thickness of washer
		/// </summary>
		public double WasherThickness
		{
			get;
			set;
		}

		/// <summary>
		/// Washer at head side of bolt assembly
		/// </summary>
		public bool WasherAtHead
		{
			get;
			set;
		}

		/// <summary>
		/// Is washer at Nut side of bolt assembly
		/// </summary>
		public bool WasherAtNut
		{
			get;
			set;
		}

		#endregion


		/// <summary>
		/// Load from library - try override properties from library find BoltAssembly by name
		/// </summary>
		public bool LoadFromLibrary { get; set; }
	}
}
