using System;
using CI.DataModel;
using System.Xml.Serialization;


namespace CI.Geometry3D
{
	/// <summary>
	/// Parabola Segment.
	/// </summary>
	public class ParabolaSegment3D : Segment3D, IArcSegment3D
	{
		#region Fields.

		/// <summary>
		/// Intermediate Point.
		/// </summary>
		private IPoint3D intermediatePoint;

		[NonSerialized]
		private ParabolaProperty parabolaProperty;

		#endregion

		#region Constructors.

		/// <summary>
		/// Initializes a new instance of <c>CI.Geometry3D.ParabolaSegment3D</c> class.
		/// </summary>
		public ParabolaSegment3D()
			: base()
		{
			Name = "ParabolaSegment";
			intermediatePoint = new Point3D();
			SubscribeEvents(intermediatePoint);
			parabolaProperty = null;
		}

		/// <summary>
		/// Segment Parabola.
		/// </summary>
		/// <param name="startPoint">Start Point</param>
		/// <param name="intermediatePoint">Intermediate Point</param>
		/// <param name="endPoint">End Point</param>
		public ParabolaSegment3D(IPoint3D startPoint, IPoint3D intermediatePoint, IPoint3D endPoint)
			: base(startPoint, endPoint)
		{
			Name = "ParabolaSegment";
			this.intermediatePoint = intermediatePoint;
			SubscribeEvents(intermediatePoint);
			parabolaProperty = null;
		}

		/// <summary>
		/// Creates an Parabola from another segment Parabola.
		/// </summary>
		/// <param name="segmentParabola">Parabola Segment</param>
		public ParabolaSegment3D(IArcSegment3D segmentParabola)
			: base(segmentParabola)
		{
			if (segmentParabola != null)
			{
				Name = (segmentParabola as ElementBase).Name;
				intermediatePoint = new Point3D(segmentParabola.IntermedPoint.X, segmentParabola.IntermedPoint.Y, segmentParabola.IntermedPoint.Z);
				SubscribeEvents(intermediatePoint);
			}

			parabolaProperty = null;
		}

		#endregion

		#region ISegment3D Properties.

		/// <summary>
		/// Segment Type.
		/// </summary>
		public override SegmentType SegmentType
		{
			get
			{
				return Geometry3D.SegmentType.Parabola;
			}
			set { }
		}

		/// <summary>
		/// Intermediate Point.
		/// </summary>
		[XmlIgnore]
		public IPoint3D IntermedPoint
		{
			get
			{
				return intermediatePoint;
			}

			set
			{
				if (intermediatePoint != value)
				{
					UnsubscribeEvents(intermediatePoint);
					intermediatePoint = value;
					SubscribeEvents(intermediatePoint);
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

		#endregion

		internal ParabolaProperty Property
		{
			get
			{
				if (parabolaProperty == null)
				{
					parabolaProperty = new ParabolaProperty(this);
				}

				return parabolaProperty;
			}
		}

		#region ISegment3D Methods.

		

		/// <summary>
		/// Create new parabola and make a copy of existing parabola.
		/// </summary>
		/// <param name="clonePoints">if true the segment3D is clonned and points are cloned too</param>
		/// <returns>Return a cloned parabola</returns>
		public override ISegment3D CloneSegment(bool clonePoints)
		{
			if (clonePoints)
			{
				return new ParabolaSegment3D(this);
			}
			else
			{
				ParabolaSegment3D newSeg = new ParabolaSegment3D(StartPoint, IntermedPoint, EndPoint);
				ICoordSystem newSystem = null;
				LocalCoordinateSystem.CopyTo(ref newSystem);
				newSeg.LocalCoordinateSystem = newSystem;
				return newSeg;
			}
		}

		/// <summary>
		/// Gets all angle change of parabola
		/// </summary>
		/// <returns>Angle change</returns>
		internal double GetAngleChange()
		{
			double angle = 0;
			if (Property.IsValid)
			{
				Vector3D vect1 = new Vector3D();
				Vector3D vect2 = new Vector3D();
				GeomOperation.GetTangentOnSegment(this, 0.0, ref vect1);
				GeomOperation.GetTangentOnSegment(this, 1.0, ref vect2);
				angle = GeomOperation.GetAngle(vect1, vect2);
			}
			else
			{
				var seg = new ArcSegment3D(StartPoint, IntermedPoint, EndPoint);
				angle = GeomOperation.GetArcAngle(seg);
			}

			return angle;
		}

		/// <summary>
		/// Subscribe Segment point events
		/// </summary>
		protected override void SubscribePointEvents()
		{
			base.SubscribePointEvents();
			SubscribeEvents(IntermedPoint);
		}

		/// <summary>
		/// On change segment call Notify all
		/// </summary>
		/// <param name="sender">Point3D</param>
		protected override void OnSegmentPointChanged(object sender)
		{
			base.OnSegmentPointChanged(sender);
			parabolaProperty = null;
		}

		#endregion
	}
}
