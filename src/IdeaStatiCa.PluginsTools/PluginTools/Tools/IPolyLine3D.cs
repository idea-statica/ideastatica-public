using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CI.Geometry3D
{
	[Guid("fd73fa5e-d90c-37c4-80db-b356f27ca1a7")]
	public interface IPolyLine3D
	{
		#region Properties

		/// <summary>
		/// Count of segments
		/// </summary>
		int Count
		{
			get;
		}

		/// <summary>
		/// Gets whether this polygon is closed or not
		/// </summary>
		bool IsClosed
		{
			get;
		}

		/// <summary>
		/// Gets the segments contained by the polygon
		/// </summary>
		IEnumerable<ISegment3D> Segments
		{
			get;
		}

		/// <summary>
		/// Gets or sets the segment at the specified index
		/// </summary>
		/// <param name="index">The zero based index of the element to get or set</param>
		/// <returns>The segment at the specified index</returns>
		ISegment3D this[int index]
		{
			get;
			set;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Add a segment to polyline
		/// </summary>
		/// <param name="segment">New segment to be added</param>
		void Add(ISegment3D segment);

		/// <summary>
		/// Remove all segments from the collection
		/// </summary>
		void Clear();

		/// <summary>
		/// Force closing the polygon
		/// </summary>
		void Close();

		/// <summary>
		/// Insert a segment to the collection at the specified index
		/// </summary>
		/// <param name="index">The zero based index at which item should be inserted</param>
		/// <param name="item">The segment to insert into the collection</param>
		void Insert(int index, ISegment3D item);

		/// <summary>
		/// Inserts <paramref name="segmentToInsert"/> after <paramref name="baseSegment"/>.
		/// If <paramref name="baseSegment"/> in not in the polyline exception is thrown.
		/// </summary>
		/// <param name="baseSegment"><paramref name="segmentToInsert"/> will be added after this segment</param>
		/// <param name="segmentToInsert">Segment to add</param>
		void InsertSegmentAfterSegment(ISegment3D baseSegment, ISegment3D segmentToInsert);

		/// <summary>
		/// Removes a specific segment from the polyline.
		/// </summary>
		/// <param name="segment">The object to remove from the polyline.</param>
		/// <returns>Index of the removed segment in the polyline.</returns>
		int Remove(ISegment3D segment);

		/// <summary>
		/// Remove the item in the collection at the specified index
		/// </summary>
		/// <param name="index">The zero based index at which item should be inserted</param>
		void RemoveAt(int index);

		/// <summary>
		/// Removes a specific segment from the polyline, adaptes shorter segment
		/// </summary>
		/// <param name="segment">The object to remove from the polyline.</param>
		/// <returns>Index of the removed segment</returns>
		int RemoveAdaptShorter(ISegment3D segment);

		#endregion
	}
}
