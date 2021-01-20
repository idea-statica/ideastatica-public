using System.Xml.Serialization;

namespace IdeaRS.OpenModel.Model
{
	/// <summary>
	/// Represents a segment in 3D space.
	/// </summary>
	[XmlInclude(typeof(RebarSegment3DLine))]
	[XmlInclude(typeof(RebarSegment3DArc3Pts))]
	public abstract class RebarSegment3DBase : OpenObject
	{
		/// <summary>
		/// Gets or sets the endpoint of the segment.
		/// </summary>
		public RebarPoint3D EndPoint { get; set; }
	}
}