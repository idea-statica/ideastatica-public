using System.Runtime.Serialization;

namespace IdeaStatiCa.Api.Connection.Model.Material
{
	/// <summary>
	/// One segment of a cross-section boundary. Segments are self-contained (each carries its own
	/// <see cref="Start"/>) and form an ordered chain: every segment's <see cref="End"/> equals the
	/// next segment's <see cref="Start"/>, and in a closed boundary the last segment's End equals
	/// the first segment's Start. Coordinates are absolute in the cross-section plane
	/// (Y horizontal, Z vertical), in meters, with the origin at the section centroid.
	/// Polymorphic on the wire: every element is one of the known subtypes and carries the <c>$type</c> discriminator.
	/// </summary>
	[KnownType(typeof(ConCssLineSegment))]
	[KnownType(typeof(ConCssArcSegment))]
	public abstract class ConCssSegment
	{

		/// <summary>Start point of the segment (equals the previous segment's End).</summary>
		public ConCssPoint2D Start { get; set; }

		/// <summary>End point of the segment (equals the next segment's Start).</summary>
		public ConCssPoint2D End { get; set; }
	}

	/// <summary>A straight segment from Start to End.</summary>
	public class ConCssLineSegment : ConCssSegment
	{
	}

	/// <summary>
	/// A circular arc from Start to End passing through <see cref="Mid"/>. Mid is an interior
	/// point of the arc — on the circle, strictly between Start and End along the arc, but not
	/// necessarily the arc's midpoint. The three points uniquely determine the circle's centre,
	/// radius and sweep direction.
	/// </summary>
	public class ConCssArcSegment : ConCssSegment
	{

		/// <summary>Interior point of the arc (on the circle, between Start and End).</summary>
		public ConCssPoint2D Mid { get; set; }
	}
}
