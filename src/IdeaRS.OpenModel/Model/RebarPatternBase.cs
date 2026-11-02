using System.Xml.Serialization;

namespace IdeaRS.OpenModel.Model
{
	/// <summary>
	/// Represents a base class for Rebar Pattern
	/// </summary>
	[XmlInclude(typeof(RebarStirrupPattern))]
	public abstract class RebarPatternBase : OpenElementId
	{
		/// <summary>
		/// Mode of defining the position of the pattern
		/// false is for absolute length (position) of the pattern (default)
		/// true is for relative length (position) of the pattern in range [0 1]
		/// </summary>
		public bool Relative { get; set; }

		/// <summary>
		/// This defines begin position of the pattern.
		/// It is the distance from the start point of the group.
		/// Its value should be in corelation to <c>PositionMode</c>
		/// </summary>
		public double StartPosition { get; set; }

		/// <summary>
		/// This defines end position of the pattern.
		/// It is the distance from the start point of the group.
		/// Its value should be in corelation to <c>PositionMode</c>
		/// </summary>
		public double EndPosition { get; set; }
	}
}
