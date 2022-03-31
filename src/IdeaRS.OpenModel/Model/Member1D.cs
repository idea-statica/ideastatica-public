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
		/// List of <see cref="Span"/>.
		/// </summary>
		public List<ReferenceElement> Spans { get; set; }

		public ReferenceElement CrossSection { get; set; }

		public Alignment Alignment { get; set; }
	}
}