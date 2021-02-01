using CI.DataModel;
using System;
using System.Xml.Serialization;

namespace CI.Geometry3D
{
	[XmlInclude(typeof(LineSegment3D))]
	[XmlInclude(typeof(ArcSegment3D))]
	[XmlInclude(typeof(ParabolaSegment3D))]
	[System.Diagnostics.DebuggerDisplay("dist=[{EndPoint.X-StartPoint.X}; {EndPoint.Y-StartPoint.Y}, {EndPoint.Z-StartPoint.Z}] Start=[{StartPoint.X}; {StartPoint.Y}; {StartPoint.Z}], End=[{EndPoint.X}; {EndPoint.Y}; {EndPoint.Z}]; {GetType().Name}")]
	public abstract class Segment3D : ElementBase, ISegment3D
	{
		#region ISegment3D Members

		/// <summary>
		/// Start Point
		/// </summary>
		private IPoint3D startPoint;

		/// <summary>
		/// EndPoint
		/// </summary>
		private IPoint3D endPoint;

		/// <summary>
		/// Local Coordinate system
		/// </summary>
		private ICoordSystem localCoordinateSystem;

		#endregion

		#region Constructors

		/// <summary>
		/// create a segment with zero length
		/// </summary>
		public Segment3D()
		{
			startPoint = new Point3D();
			endPoint = new Point3D();
			SubscribePointEvents();
		}

		/// <summary>
		/// Create a segment from two points
		/// </summary>
		/// <param name="startPoint">Starting point</param>
		/// <param name="endPoint">Ending point</param>
		public Segment3D(IPoint3D startPoint, IPoint3D endPoint)
		{
			this.startPoint = startPoint;
			this.endPoint = endPoint;
			SubscribePointEvents();
		}

		/// <summary>
		/// Create a segment as a copy of another
		/// </summary>
		/// <param name="source">Source segment</param>
		public Segment3D(ISegment3D source)
		{
			if (source != null)
			{
				startPoint = new Point3D(source.StartPoint.X, source.StartPoint.Y, source.StartPoint.Z);
				endPoint = new Point3D(source.EndPoint.X, source.EndPoint.Y, source.EndPoint.Z);
				(source as Segment3D).LocalCoordinateSystem.CopyTo(ref localCoordinateSystem);
				SubscribePointEvents();
			}
		}

		#endregion

		#region ISegment3D Members (ale v ISegmentu3D nejsou ...)

		/// <summary>
		/// Local coordinate system
		/// </summary>
		[XmlIgnore]
		public ICoordSystem LocalCoordinateSystem
		{
			get
			{
				if (localCoordinateSystem == null)
				{
					localCoordinateSystem = (ICoordSystem)new CoordSystemByZup();
				}

				return localCoordinateSystem;
			}

			set
			{
				localCoordinateSystem = value;
			}
		}

		/// <summary>
		/// Get the type of Segment
		/// </summary>
		public abstract SegmentType SegmentType
		{
			get;
			set;
		}

		/// <summary>
		/// gets or sets the start point of the segment
		/// </summary>
		[XmlIgnore]
		public IPoint3D StartPoint
		{
			get
			{
				return startPoint;
			}

			set
			{
				if (startPoint != value)
				{
					UnsubscribeEvents(startPoint);
					startPoint = value;
					SubscribeEvents(startPoint);
					OnSegmentPointChanged(this);
				}
			}
		}

		/// <summary>
		/// gets or sets the end point of the segment
		/// </summary>
		[XmlIgnore]
		public IPoint3D EndPoint
		{
			get
			{
				return endPoint;
			}

			set
			{
				if (endPoint != value)
				{
					UnsubscribeEvents(endPoint);
					endPoint = value;
					SubscribeEvents(endPoint);
					OnSegmentPointChanged(this);
				}
			}
		}

		/// <summary>
		/// StartPoint
		/// </summary>
		[XmlElement(ElementName = "StartPoint")]
		public Point3D StartPointSerialize
		{
			get { return StartPoint as Point3D; }
			set { StartPoint = value; }
		}

		/// <summary>
		/// EndPoint
		/// </summary>
		[XmlElement(ElementName = "EndPoint")]
		public Point3D EndPointSerialize
		{
			get { return EndPoint as Point3D; }
			set { EndPoint = value; }
		}

		/// <summary>
		/// Creates a new segment from this segment
		/// </summary>
		/// <param name="clonePoints">if true the segment3D is clonned and points are cloned too</param>
		/// <returns>a new ISegment3D object</returns>
		public abstract ISegment3D CloneSegment(bool clonePoints = true);

		/// <summary>
		/// SubscribeEvents
		/// </summary>
		/// <param name="point">Point</param>
		protected void SubscribeEvents(IPoint3D point)
		{
			////UnsubscribeEvents(point);
			//ElementBase elementPoint = point as ElementBase;
			//if (elementPoint != null)
			//{
			//  //elementPoint.ObjectChanged += new ObjectChangedEventHandler(OnSegmentPointChanged);
			//  //elementPoint.ObjectLinked += new ObjectLinkedEventHandler(OnObjectLinked);
			//  PropertyChangedEventManager.AddListener(elementPoint, this);
			//}
		}

		/// <summary>
		/// SubscribeEvents
		/// </summary>
		/// <param name="point">Point</param>
		protected void UnsubscribeEvents(IPoint3D point)
		{
			//ElementBase elementPoint = point as ElementBase;
			//if (elementPoint != null)
			//{
			//  //elementPoint.ObjectChanged -= new ObjectChangedEventHandler(OnSegmentPointChanged);
			//  //elementPoint.ObjectLinked -= new ObjectLinkedEventHandler(OnObjectLinked);
			//  PropertyChangedEventManager.RemoveListener(elementPoint, this);
			//}
		}

		/// <summary>
		/// Subscribe Segment point events
		/// </summary>
		protected virtual void SubscribePointEvents()
		{
			SubscribeEvents(startPoint);
			SubscribeEvents(endPoint);
		}

		///// <summary>
		///// Add Linked objects into the list
		///// </summary>
		///// <param name="sender">Sender</param>
		///// <param name="linkedObjects">Add linked objects to this list</param>
		//protected override void OnObjectLinked(object sender, System.Collections.Generic.IList<IModelItem> linkedObjects)
		//{
		//  RaiseObjectLinkedEvent(this, linkedObjects);
		//}

		/// <summary>
		/// On change segment call Notify all
		/// </summary>
		/// <param name="sender">Point3D</param>
		protected virtual void OnSegmentPointChanged(object sender)
		{
			NotifyAllChanges();
		}


		#endregion
	}
}