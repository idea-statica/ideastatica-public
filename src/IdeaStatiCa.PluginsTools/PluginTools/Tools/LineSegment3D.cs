
using CI.DataModel;

namespace CI.Geometry3D
{
	public class LineSegment3D : Segment3D, ILineSegment3D
	{
		#region Constructors

		/// <summary>
		/// crerates line with zero length
		/// </summary>
		public LineSegment3D()
			: base()
		{
			Name = "LineSegment";
		}

		/// <summary>
		/// Create a Line Segment from two Point3D objects
		/// </summary>
		/// <param name="startPoint">Starting point</param>
		/// <param name="endPoint">Ending point</param>
		public LineSegment3D(IPoint3D startPoint, IPoint3D endPoint)
			: base(startPoint, endPoint)
		{
			Name = "LineSegment";
		}

		/// <summary>
		/// Create a Line segment from another line segment
		/// </summary>
		/// <param name="source">Source line segment</param>
		public LineSegment3D(ILineSegment3D source) :
			base(source)
		{
			Name = (source as ElementBase).Name;
		}

		#endregion

		#region ISegment3D Properties

		/// <summary>
		/// Gets the type of the segment.
		/// </summary>
		public override SegmentType SegmentType
		{
			get
			{
				return SegmentType.Line;
			}
			set { }
		}

		#endregion

		#region ISegment3D Members

		public override ISegment3D CloneSegment(bool clonePoints)
		{
			if (clonePoints)
			{
				return new LineSegment3D(this);
			}
			else
			{
				LineSegment3D newSeg = new LineSegment3D(StartPoint, EndPoint);
				ICoordSystem newSystem = null;
				LocalCoordinateSystem.CopyTo(ref newSystem);
				newSeg.LocalCoordinateSystem = newSystem;
				return newSeg;
			}
		}

		#endregion
	}
}