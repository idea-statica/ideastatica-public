using System.Collections.Generic;

namespace IdeaStatiCa.BimApi
{
	/// <summary>
	/// Represents a polyline in three-dimensional space.
	/// </summary>
	public interface IIdeaPolyLine3D : IIdeaObject
	{
		/// <summary>
		/// Gets the segments contained by the polygon
		/// </summary>
		List<IIdeaSegment3D> Segments { get; }
	}
}
