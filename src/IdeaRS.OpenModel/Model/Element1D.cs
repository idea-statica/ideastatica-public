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
		/// Local eccentricity X at the begin of Element1D
		/// </summary>
		public double EccentricityBeginX { get; set; }

		/// <summary>
		/// Local eccentricity Y at the begin of Element1D
		/// </summary>
		public double EccentricityBeginY { get; set; }

		/// <summary>
		///  Local eccentricity Z at the begin of Element1D
		/// </summary>
		public double EccentricityBeginZ { get; set; }

		/// <summary>
		///  Local eccentricity X at the end of Element1D
		/// </summary>
		public double EccentricityEndX { get; set; }

		/// <summary>
		///  Local eccentricity Y at the end of Element1D
		/// </summary>
		public double EccentricityEndY { get; set; }

		/// <summary>
		///  Local eccentricity Z at the end of Element1D
		/// </summary>
		public double EccentricityEndZ { get; set; }
	}
}