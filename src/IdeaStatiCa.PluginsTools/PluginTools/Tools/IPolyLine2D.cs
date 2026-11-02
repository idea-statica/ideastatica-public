using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;

namespace CI.Geometry2D
{
	[ComVisible(false)]
	public interface IPolyLine2D : ICloneable
	{
		/// <summary>
		/// Gets or sets the point where the <c>IPolyLine2D</c> begins.
		/// </summary>
		IdaComPoint2D StartPoint { get; set; }

		/// <summary>
		/// Gets a value that specifies whether this polylines first and last segments are connected.
		/// </summary>
		bool IsClosed { get; }

		/// <summary>
		/// Gets segments of <c>IPolyLine2D</c>.
		/// </summary>
		IList<ISegment2D> Segments { get; }

		/// <summary>
		/// Gets the length of this <c>IPolyLine2D</c> object.
		/// </summary>
		double Length { get; }

		/// <summary>
		/// Gets a <c>Rect</c> that specifies the bounding box of this graphical object.
		/// </summary>
		Rect Bounds { get; }

		/// <summary>
		/// polyline Id
		/// </summary>
		int Id { get; set; }

		/// <summary>
		/// Close <c>IPolyLine2D</c> by a LineSegment.
		/// </summary>
		void Close();
	}

	[ComVisible(true)]
	public interface IPolyLine2DCom
	{
		/// <summary>
		/// Gets or sets the point where the PolyLine2D begins.
		/// </summary>
		IdaComPoint2D StartPoint { get; set; }

		/// <summary>
		/// Gets a value that specifies whether this polylines first and last segments are connected.
		/// </summary>
		bool IsClosed { get; }

		/// <summary>
		/// Gets segments of <c>IPolyLine2D</c>.
		/// </summary>
		IList SegmentsCom { get; }
	}
}
