using IdeaRS.OpenModel.Geometry3D;
using System.Collections.Generic;

namespace IdeaRS.OpenModel.Model
{
	/// <summary>
	/// Type of Member 1D
	/// </summary>
	public enum Member1DType
	{
		/// <summary>
		/// Beam member
		/// </summary>
		Beam,

		/// <summary>
		/// Column member
		/// </summary>
		Column,

		/// <summary>
		/// Truss member
		/// </summary>
		Truss,

		/// <summary>
		/// Rib member
		/// </summary>
		Rib,

		/// <summary>
		/// Beam slab, rcs element
		/// </summary>
		BeamSlab,
	}

	public enum Alignment
	{
		Center,
		Top,
		Bottom,
		Left,
		Right,
		TopLeft,
		TopRight,
		BottomLeft,
		BottomRight
	}

	/// <summary>
	/// Representation of member1D
	/// </summary>
	[OpenModelClass("CI.StructModel.Structure.Member1D,CI.StructuralElements", "CI.StructModel.Structure.IMember1D,CI.BasicTypes")]
	public class Member1D : OpenElementId
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public Member1D()
		{
			Elements1D = new List<ReferenceElement>();
		}

		/// <summary>
		/// Name of Element
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Array of element1D
		/// </summary>
		/// <returns>List of Element1D</returns>
		public List<ReferenceElement> Elements1D { get; set; }

		/// <summary>
		/// Beam, column,...
		/// </summary>
		public Member1DType Member1DType { get; set; }

		/// <summary>
		/// Gets, sets hinge located in the begining of element
		/// </summary>
		public ReferenceElement HingeBegin { get; set; }

		/// <summary>
		/// Gets, sets hinge located in the end of element
		/// </summary>
		public ReferenceElement HingeEnd { get; set; }

		/// <summary>
		/// <see cref="Model.Taper"/> for defining haunched member.
		/// </summary>
		public ReferenceElement Taper { get; set; }

		/// <summary>
		/// <see cref="CrossSection.CrossSection">Cross-section</see> of the member.
		/// </summary>
		public ReferenceElement CrossSection { get; set; }

		/// <summary>
		/// Alignment for haunched member.
		/// </summary>
		public Alignment Alignment { get; set; }

		/// <summary>
		/// Mirrors the cross-section of this member on the Y-axis.
		/// </summary>
		public bool MirrorY { get; set; }

		/// <summary>
		/// Mirrors the cross-section of this member on the Z-axis.
		/// </summary>
		public bool MirrorZ { get; set; }

		/// <summary>
		/// An eccentricity at the begin of the Member1D. Coordinate system can be specified by <see cref="EccentricityReference"/>.
		/// </summary>
		public Vector3D EccentricityBegin { get; set; } = new Vector3D();

		/// <summary>
		/// An eccentricity at the end of the Member1D. Coordinate system can be specified by <see cref="EccentricityReference"/>.
		/// </summary>
		public Vector3D EccentricityEnd { get; set; } = new Vector3D();

		/// <summary>
		/// An insertion point of the member relative to its reference line (e.g., top-of-steel, bottom-of-steel).
		/// </summary>
		public InsertionPoints InsertionPoint { get; set; }

		/// <summary>
		/// A reference system (local or global) in which <see cref="EccentricityBegin"/> and <see cref="EccentricityEnd"/> are defined.
		/// </summary>
		public EccentricityReference EccentricityReference { get; set; } = EccentricityReference.LocalCoordinateSystem;
	}
}