using CI.DataModel;
using System.Xml.Serialization;

namespace CI.Geometry3D
{
	public class ArcSegment3D : Segment3D, IArcSegment3D
	{
		#region Fields

		private IPoint3D intMedPoint;

		#endregion

		#region Constructors
		/// <summary>
		/// Creates an arc segment of zero length
		/// </summary>
		public ArcSegment3D()
			: base()
		{
			Name = "ArcSegment";
			intMedPoint = new Point3D();
			SubscribeEvents(intMedPoint);
		}

		/// <summary>
		/// creates an arc segment from three points
		/// </summary>
		/// <param name="startPoint">Starting point</param>
		/// <param name="intMedPoint">Intermediate point</param>
		/// <param name="endPoint">Ending point</param>
		public ArcSegment3D(IPoint3D startPoint, IPoint3D intMedPoint, IPoint3D endPoint)
			: base(startPoint, endPoint)
		{
			Name = "ArcSegment";
			this.intMedPoint = intMedPoint;
			SubscribeEvents(this.intMedPoint);
		}

		/// <summary>
		/// Creates an arc segment from another arc segment.
		/// </summary>
		/// <param name="source">Source segment</param>
		public ArcSegment3D(IArcSegment3D source)
			: base(source)
		{
			if (source != null)
			{
				Name = (source as ElementBase).Name;
				intMedPoint = new Point3D(source.IntermedPoint.X, source.IntermedPoint.Y, source.IntermedPoint.Z);
				SubscribeEvents(this.intMedPoint);
			}
		}

		#endregion

		#region IArcSegment3D Properties

		/// <summary>
		/// gets or sets the inter mediate point of an arc segment
		/// </summary>
		[XmlIgnore]
		public IPoint3D IntermedPoint
		{
			get
			{
				return intMedPoint;
			}

			set
			{
				if (intMedPoint != value)
				{
					UnsubscribeEvents(intMedPoint);
					intMedPoint = value;
					SubscribeEvents(intMedPoint);
					OnSegmentPointChanged(this);
				}
			}
		}

		/// <summary>
		/// IntermedPoint
		/// </summary>
		[XmlElement(ElementName = "IntermedPoint")]
		public Point3D IntermedPointSerialize
		{
			get { return IntermedPoint as Point3D; }
			set { IntermedPoint = value; }
		}

		/// <summary>
		/// Gets the type of the segment.
		/// </summary>
		public override SegmentType SegmentType
		{
			get
			{
				return SegmentType.CircularArc;
			}
			set { }
		}

		#endregion

		#region ISegment3D Methods

		public override ISegment3D CloneSegment(bool clonePoints)
		{
			if (clonePoints)
			{
				return new ArcSegment3D(this);
			}
			else
			{
				ArcSegment3D newSeg = new ArcSegment3D(StartPoint, IntermedPoint, EndPoint);
				ICoordSystem newSystem = null;
				LocalCoordinateSystem.CopyTo(ref newSystem);
				newSeg.LocalCoordinateSystem = newSystem;
				return newSeg;
			}
		}

		/// <summary>
		/// Subscribe Segment point events
		/// </summary>
		protected override void SubscribePointEvents()
		{
			base.SubscribePointEvents();
			SubscribeEvents(IntermedPoint);
		}

		#endregion
	}
}