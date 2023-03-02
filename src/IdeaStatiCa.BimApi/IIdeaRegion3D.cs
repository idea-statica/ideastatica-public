using IdeaRS.OpenModel;
using System.Collections.Generic;

namespace IdeaStatiCa.BimApi
{
	/// <summary>
	/// Represents a region in three-dimensional space included outline (border) and openings.
	/// </summary>
	public interface IIdeaRegion3D : IIdeaObject
	{
		/// <summary>
		/// Gets the outline polyline of the region
		/// </summary>
		IIdeaPolyLine3D Outline { get; }

		/// <summary>
		/// Gets the hole polygons inside the region
		/// </summary>
		List<IIdeaPolyLine3D> Openings { get; }
	}
}
