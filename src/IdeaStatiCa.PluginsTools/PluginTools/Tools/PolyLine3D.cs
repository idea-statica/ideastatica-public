using System;
using System.Collections.Generic;
using CI.DataModel;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Collections;

namespace CI.Geometry3D
{
	public class PolyLine3D : ElementBase, IPolyLine3D
	{
		#region Fields

		/// <summary>
		/// List of segments
		/// </summary>
		private IList<ISegment3D> segments;

		#endregion

		#region Constructor

		/// <summary>
		/// Creates a polyline of zero length
		/// </summary>
		public PolyLine3D()
		{
			Name = "PolyLine";
			segments = new List<ISegment3D>();
		}

		/// <summary>
		/// Creates a polyline from a List of segments
		/// </summary>
		/// <param name="items">List of segments</param>
		public PolyLine3D(IList<ISegment3D> items)
		{
			Name = "PolyLine";
			segments = new List<ISegment3D>();
			if (items != null)
			{
				foreach (ISegment3D o in items)
				{
					Add(o.CloneSegment());
				}
			}
		}

		/// <summary>
		/// creates a polyline from another polyline
		/// </summary>
		/// <param name="source">Source polyline</param>
		public PolyLine3D(IPolyLine3D source)
		{
			if (source != null)
			{
				Name = (source as ElementBase).Name;
				segments = new List<ISegment3D>();

				if (source.Segments != null)
				{
					IPoint3D point = null;
					foreach (ISegment3D segment in source.Segments)
					{
						ISegment3D seg = segment.CloneSegment();
						if (point != null)
						{
							seg.StartPoint = point;
						}

						point = seg.EndPoint;
						Add(seg);
					}

					if (source.IsClosed && segments. Count > 0)
					{
						segments[segments.Count - 1].EndPoint = segments[0].StartPoint;
					}
				}
			}
		}

		#endregion

		#region IPolyline3D Properties

		/// <summary>
		/// Count of segments
		/// </summary>
		[XmlIgnore]
		public int Count
		{
			get
			{
				if (segments == null)
				{
					return 0;
				}

				return segments.Count;
			}
		}

		/// <summary>
		/// gets whether this polygon is closed
		/// </summary>
		[XmlIgnore]
		public bool IsClosed
		{
			get
			{
				int count = segments.Count;
				if (count == 0)
				{
					return false;
				}
				else if (count == 1)
				{
					if (segments[0].SegmentType == SegmentType.Line)
					{
						return false;
					}
				}

				return GeomOperation.IsEqual(segments[0].StartPoint, segments[count - 1].EndPoint);
			}
		}

		/// <summary>
		/// Gets or the segments inside the polyline
		/// </summary>
		[XmlIgnore]
		public IEnumerable<ISegment3D> Segments
		{
			get
			{
				return segments;
			}
		}

		/// <summary>
		/// Gets or sets the segment at the specified index
		/// </summary>
		/// <param name="index">The zero based index of the element to get or set</param>
		/// <returns>The segment at the specified index</returns>
		public ISegment3D this[int index]
		{
			get
			{
				return segments[index];
			}

			set
			{
				UnsubscribeEventsFromSegment(segments[index]);
				segments[index] = value;
				SubscribeEventsFromSegment(segments[index]);
			}
		}


		/// <summary>
		/// Gets segments of <c>PolyLine3D</c>.
		/// </summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[XmlArray(ElementName = "Segments")]
		[XmlArrayItem(Type = typeof(Segment3D), ElementName = "Segment")]
		public IList SegmentsCom
		{
			get { return segments as IList; }
		}

		#endregion

		#region IPolyline3D Methods

		/// <summary>
		/// Add a segment to polyline
		/// </summary>
		/// <param name="segment">New segment to be added</param>
		public void Add(ISegment3D segment)
		{
			segments.Add(segment);
			SubscribeEventsFromSegment(segment);
		}

		/// <summary>
		/// Remove all segments from the collection
		/// </summary>
		public void Clear()
		{
			foreach (ISegment3D segment in segments)
			{
				UnsubscribeEventsFromSegment(segment);
			}

			segments.Clear();
		}

		/// <summary>
		/// Force the polygon to close with a line segment
		/// </summary>
		public void Close()
		{
			if (IsClosed == false)
			{
				if (segments.Count > 0)
				{
					IPoint3D last = segments[segments.Count - 1].EndPoint;
					IPoint3D first = segments[0].StartPoint;
					Add(new LineSegment3D(last, first));
				}
			}
		}

		/// <summary>
		/// Insert a segment to the collection at the specified index
		/// </summary>
		/// <param name="index">The zero based index at which item should be inserted</param>
		/// <param name="item">The segment to insert into the collection</param>
		public void Insert(int index, ISegment3D item)
		{
			segments.Insert(index, item);
			SubscribeEventsFromSegment(item);
		}

		/// <summary>
		/// Inserts <paramref name="segmentToInsert"/> after <paramref name="baseSegment"/>.
		/// If <paramref name="baseSegment"/> in not in the polyline exception is thrown.
		/// </summary>
		/// <param name="baseSegment"></param>
		/// <param name="segmentToInsert"></param>
		public void InsertSegmentAfterSegment(ISegment3D baseSegment, ISegment3D segmentToInsert)
		{
			int baseSegmentIndex = segments.IndexOf(baseSegment);
			if (baseSegmentIndex == -1)
			{
				throw new ArgumentException("PolyLine3D InsertAfter : Segment not found");
			}

			baseSegment.EndPoint = segmentToInsert.StartPoint;

			segments.Insert(baseSegmentIndex + 1, segmentToInsert);
			SubscribeEventsFromSegment(segmentToInsert);
		}

		/// <summary>
		/// Remove the item in the collection at the specified index
		/// </summary>
		/// <param name="index">The zero based index at which item should be inserted</param>
		public void RemoveAt(int index)
		{
			int segmentCount = segments.Count;

			if (index == -1)
			{
				throw new ArgumentException("Segment is not included in polyline");
			}

			bool isClosed = IsClosed;

			//if (isClosed || !(index == 0) || !(index == segmentCount - 1))
			if (isClosed || ((index != 0) && (index != segmentCount - 1)))
			{
				if (segmentCount > 2)
				{
					ISegment3D prevSegment;
					ISegment3D nextSegment;

					if (index == 0)
					{
						prevSegment = segments[segmentCount - 1];
						nextSegment = segments[1];
					}
					else if (index == segmentCount - 1)
					{
						prevSegment = segments[index - 1];
						nextSegment = segments[0];
					}
					else
					{
						prevSegment = segments[index - 1];
						nextSegment = segments[index + 1];
					}

					prevSegment.EndPoint = nextSegment.StartPoint;
				}
			}

			UnsubscribeEventsFromSegment(segments[index]);
			segments.RemoveAt(index);
		}

		/// <summary>
		/// Removes a specific segment from the polyline.
		/// </summary>
		/// <param name="segment">The object to remove from the polyline.</param>
		/// <returns>Index of the removed segment</returns>
		public int Remove(ISegment3D segment)
		{
			UnsubscribeEventsFromSegment(segment);
			int indexOfDelSegment = segments.IndexOf(segment);
			RemoveAt(indexOfDelSegment);
			return indexOfDelSegment;
		}

 		/// <summary>
		/// Remove segment[index], adaptes geometry so that shorter segment changes positions
		/// </summary>
		/// <param name="index">Segment index</param>
		public void RemoveAtAdaptShorter(int index)
		{
			int segmentCount = segments.Count;

			if (index == -1)
			{
				throw new ArgumentException("Segment is not included in polyline");
			}

			bool isClosed = IsClosed;

			//if (isClosed || !(index == 0) || !(index == segmentCount - 1))
			if (isClosed || ((index != 0) && (index != segmentCount - 1)))
			{
				if (segmentCount > 2)
				{
					ISegment3D prevSegment;
					ISegment3D nextSegment;

					if (index == 0)
					{
						prevSegment = segments[segmentCount - 1];
						nextSegment = segments[1];
					}
					else if (index == segmentCount - 1)
					{
						prevSegment = segments[index - 1];
						nextSegment = segments[0];
					}
					else
					{
						prevSegment = segments[index - 1];
						nextSegment = segments[index + 1];
					}

					double prevLength = GeomOperation.GetLength(prevSegment);
					double nextLength = GeomOperation.GetLength(nextSegment);

					if (prevLength < nextLength)
					{
						prevSegment.EndPoint = nextSegment.StartPoint;
					}
					else
					{
						nextSegment.StartPoint = prevSegment.EndPoint;
					}
				}
			}

			UnsubscribeEventsFromSegment(segments[index]);
			segments.RemoveAt(index);
		}

		/// <summary>
		/// Removes a specific segment from the polyline, adaptes shorter segment
		/// </summary>
		/// <param name="segment">The object to remove from the polyline.</param>
		/// <returns>Index of the removed segment</returns>
		public int RemoveAdaptShorter(ISegment3D segment)
		{
			UnsubscribeEventsFromSegment(segment);
			int indexOfDelSegment = segments.IndexOf(segment);
			RemoveAtAdaptShorter(indexOfDelSegment);
			return indexOfDelSegment;
		}

		#endregion

		#region PolyLine3D Methods

		/// <summary>
		/// Subscribe events from all segments
		/// </summary>
		protected void SubscribeEventsFromSegments()
		{
			foreach (ISegment3D segment in Segments)
			{
				SubscribeEventsFromSegment(segment);
			}
		}

		/// <summary>
		/// Subscribe Events From given segment
		/// </summary>
		/// <param name="segment"></param>
		protected void SubscribeEventsFromSegment(ISegment3D segment)
		{
			//ElementBase elementSegment = segment as ElementBase;
			//if (elementSegment == null)
			//{
			//  return;
			//}

			////UnsubscribeEventsFromSegment(segment);
			////elementSegment.ObjectChanged += new ObjectChangedEventHandler(OnGeometryChanged);
			////elementSegment.ObjectLinked += new ObjectLinkedEventHandler(OnObjectLinked);
			//PropertyChangedEventManager.AddListener(elementSegment, this);
		}

		/// <summary>
		/// Unsubscribe Events From given segment
		/// </summary>
		/// <param name="segment"></param>
		protected void UnsubscribeEventsFromSegment(ISegment3D segment)
		{
			//ElementBase elementSegment = segment as ElementBase;
			//if (elementSegment == null)
			//{
			//  return;
			//}

			////elementSegment.ObjectChanged -= new ObjectChangedEventHandler(OnGeometryChanged);
			////elementSegment.ObjectLinked -= new ObjectLinkedEventHandler(OnObjectLinked);
			//PropertyChangedEventManager.RemoveListener(elementSegment, this);
		}
		#endregion
	}
}