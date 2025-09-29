using IdeaRS.OpenModel.Geometry3D;
using System;

namespace IdeaRS.OpenModel.Model
{
	/// <summary>
	/// Representation of element1D
	/// </summary>
	[OpenModelClass("CI.StructModel.Structure.Element1D,CI.StructuralElements", "CI.StructModel.Structure.IElement1D,CI.BasicTypes")]
	public class Element1D : OpenElementId
	{
		/// <summary>
		/// Name of Element
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets, sets cross section located in the begining of this element1D
		/// </summary>
		//[Obsolete("Element cross-section are obsolete, use member cross-section or member spans for haunched members.")]
		public ReferenceElement CrossSectionBegin { get; set; }

		/// <summary>
		/// Gets, sets cross section located at the end of this element1D
		/// </summary>
		///
		//[Obsolete("Element cross-section are obsolete, use member cross-section or member spans for haunched members.")]
		public ReferenceElement CrossSectionEnd { get; set; }

		/// <summary>
		/// Line segment with start point and end point
		/// </summary>
		[OpenModelProperty("LineSegment")]
		public ReferenceElement Segment { get; set; }

		/// <summary>
		/// Rotation of Cross-section of Element1D. Difference from default Line LCS
		/// </summary>
		public double RotationRx { get; set; }
		/// <summary>
		/// Local eccentricity at the begin of Element1D. Used only for export.
		/// </summary>
		public Vector3D EccentricityBegin { get; set; } = new Vector3D();

		/// <summary>
		/// Local eccentricity at the end of Element1D. Used only for export.
		/// </summary>
		public Vector3D EccentricityEnd { get; set; } = new Vector3D();
	}
}