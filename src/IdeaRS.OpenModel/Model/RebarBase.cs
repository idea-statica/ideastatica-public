using System.Xml.Serialization;

namespace IdeaRS.OpenModel.Model
{
	/// <summary>
	/// Represents a base class for Rebars in 3D space.
	/// </summary>
	[XmlInclude(typeof(RebarSingle))]
	[XmlInclude(typeof(RebarStirrups))]
	[XmlInclude(typeof(RebarGeneral))]
	public abstract class RebarBase : OpenElementId
	{
		/// <summary>
		/// Gets or sets the reference of material properties.
		/// </summary>
		public ReferenceElement Material { get; set; }

		/// <summary>
		/// Gets or sets the Rebar Diameter.
		/// </summary>
		public double Diameter { get; set; }

		/// <summary>
		/// Gets or sets the geometrical shape of the rebar.
		/// <c>IdeaRS.OpenModel.Model.RebarShape</c>
		/// </summary>
		public ReferenceElement RebarShape { get; set; }

		/// <summary>
		/// Gets or sets the reference element of the geometry of the reference line (line along which the rebars are generated).
		/// It can be <c>IdeaRS.OpenModel.Geometry3D.Segment3D</c> or <c>IdeaRS.OpenModel.Geometry3D.PolyLine3D</c> or <c>IdeaRS.OpenModel.Model.Member1D</c> or <c>IdeaRS.OpenModel.Model.Element1D</c>
		/// </summary>
		public ReferenceElement RebarParentElement { get; set; }

		/// <summary>
		/// the element where this rebar belongs (part of that beam)
		/// </summary>
		public ReferenceElement RebarAllocationElement { get; set; }

		/// <summary>
		/// identity for group identification number
		/// </summary>
		public int GroupId { get; set; }
	}
}
