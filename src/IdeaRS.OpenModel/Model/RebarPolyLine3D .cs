using System.Collections.Generic;

namespace IdeaRS.OpenModel.Model
{
	/// <summary>
	/// Represents a polyline in 3D space.
	/// </summary>
	public class RebarPolyLine3D : OpenObject
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public RebarPolyLine3D()
		{
			Segments = new List<RebarSegment3DBase>();
		}

		/// <summary>
		/// Gets or sets the point where the <c>RebarPolyLine3D</c> begins.
		/// </summary>
		public RebarPoint3D StartPoint { get; set; }

		/// <summary>
		/// Gets segments of <c>RebarPolyLine3D</c>.
		/// </summary>
		public List<RebarSegment3DBase> Segments { get; set; }
	}
}