using System;
using System.Diagnostics;
using System.Windows;

namespace CI.Geometry2D
{
	/// <summary>
	/// Represents the intersection of two 2D segments.
	/// </summary>
	[DebuggerDisplay("RelPosCut={RelPosOnCutLine}, Intersection={Intersection}")]
	public class IntersectionData : IComparable
	{
		/// <summary>
		/// Relative position on cutting line.
		/// </summary>
		public double RelPosOnCutLine { get; set; }

		/// <summary>
		/// The relative position on the segment from start point.
		/// </summary>
		public double RelPosOnSegment { get; set; }

		/// <summary>
		/// The start point of a segment.
		/// </summary>
		public Point StartPoint { get; set; }

		/// <summary>
		/// Cutting segment.
		/// </summary>
		public ISegment2D Segment { get; set; }

		/// <summary>
		/// The intersect point.
		/// </summary>
		public Point Intersection { get; set; }

		/// <summary>
		/// Gets the angle, in degrees, between cutted segment and cut line.
		/// </summary>
		public double Angle { get; set; }

		/// <summary>
		/// Gets the angle, in degrees, between cutted segment and cut line.
		/// Angle is in interval 0 - 90 degrees.
		/// </summary>
		public double Angle090
		{
			get
			{
				var angle = Math.Abs(Angle);
				if (angle > 90)
				{
					angle = 180 - angle;
				}

				return angle;
			}
		}

		#region IComparable

		/// <summary>
		/// Compares the current instance with another object of the same type and returns
		/// an integer that indicates whether the current instance precedes, follows,
		/// or occurs in the same position in the sort order as the other object.
		/// </summary>
		/// <param name="obj">An object to compare with this instance.</param>
		/// <returns>
		/// A value that indicates the relative order of the objects being compared.
		/// The return value has these meanings: Value Meaning Less than zero This instance
		/// is less than obj. Zero This instance is equal to obj. Greater than zero This
		/// instance is greater than obj.
		/// </returns>
		public int CompareTo(object obj)
		{
			var data = obj as IntersectionData;
			if (obj != null)
			{
				return RelPosOnCutLine.CompareTo(data.RelPosOnCutLine);
			}
			else
			{
				throw new ArgumentException("Object obj is not a type of IntersectionData");
			}
		}

		#endregion IComparable
	}
}