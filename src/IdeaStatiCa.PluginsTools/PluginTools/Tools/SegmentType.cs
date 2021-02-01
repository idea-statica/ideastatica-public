
namespace CI.Geometry3D
{
	public enum SegmentType
	{
		/// <summary>
		/// POS - EF serialization
		/// </summary>
		Unknown = 0,

		/// <summary>
		/// For Line segment
		/// </summary>
		Line = 1,

		/// <summary>
		/// For Circular arc segment
		/// </summary>
		CircularArc,

		/// <summary>
		/// Parabola
		/// </summary>
		Parabola
	}
}
