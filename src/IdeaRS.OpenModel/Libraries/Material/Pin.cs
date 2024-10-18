using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.Material
{
	/// <summary>
	/// Pin
	/// </summary>
	[OpenModelClass("CI.StructModel.Libraries.Material.Pin,CI.Material", "CI.StructModel.Libraries.Material.IPin,CI.BasicTypes", typeof(Pin))]
	[DataContract]
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class Pin : OpenElementId
	{
		/// <summary>
		/// Name of pin
		/// </summary>
		public string Name { get; set; }


		/// <summary>
		/// Material of pin
		/// </summary>
		public ReferenceElement Material { get; set; }

		/// <summary>
		/// Pin diameter
		/// </summary>
		public double Diameter
		{
			get;
			set;
		}

		/// <summary>
		/// Size of hole Diameter
		/// </summary>
		public double HoleDiameter
		{
			get;
			set;
		}

		/// <summary>
		/// Has Pin Cap
		/// </summary>
		public bool HasPinCap
		{
			get;
			set;
		}

		/// <summary>
		/// Pin Cap Diameter
		/// </summary>
		public double PinCapDiameter
		{
			get;
			set;
		}

		/// <summary>
		/// Thickness of Pin Cap
		/// </summary>
		public double PinCapThickness
		{
			get;
			set;
		}

		/// <summary>
		/// Pin Overlap
		/// </summary>
		public double PinOverlap
		{
			get;
			set;
		}

		/// <summary>
		/// Load from library - try override properties from library find Pin by name
		/// </summary>
		public bool LoadFromLibrary { get; set; }
	}
}
