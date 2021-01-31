using System.Runtime.InteropServices;
namespace CI.Geometry3D
{
	[Guid("e2d501ae-701c-3900-848b-4516630ab5af")]
	public interface ISegment3D
	{
		/// <summary>
		/// Start point of the segment
		/// </summary>
		IPoint3D StartPoint { get; set; }

		/// <summary>
		/// End point of the segment
		/// </summary>
		IPoint3D EndPoint { get; set; }

		/// <summary>
		/// Gets and sets the type of the segment.
		/// </summary>
		SegmentType SegmentType { get; }

		/// <summary>
		/// Create a new segment from this segment
		/// </summary>
		/// <param name="clonePoints">if true the segment3D is clonned and points are cloned too</param>
		/// <returns>Return a cloned segment</returns>
		ISegment3D CloneSegment(bool clonePoints = true);
	}
}
