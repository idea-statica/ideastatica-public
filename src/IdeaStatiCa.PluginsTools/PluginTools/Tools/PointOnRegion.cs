using System;
using CI.DataModel;

namespace CI.Geometry3D
{
	/// <summary>
	/// Geometry For Point On Region.
	/// </summary>
	public class PointOnRegion : ElementBase, IPointOnRegion
	{
		#region Member Variables.

		/// <summary>
		/// Region
		/// </summary>
		private IRegion3D region;

		#endregion

		#region Constructors.

		/// <summary>
		/// Default constructor
		/// </summary>
		public PointOnRegion()
		{
			Name = "PointOnRegion";
			region = new Region3D();
			SubscribeEventsFromRegion();
		}

		/// <summary>
		/// Point On Region
		/// </summary>
		/// <param name="region">Region</param>
		/// <param name="offsetX">Local Offset X</param>
		/// <param name="offsetY">Local Offset Y</param>
		public PointOnRegion(IRegion3D region, double offsetX, double offsetY)
		{
			Name = "PointOnRegion";
			this.region = region;
			OffsetX = offsetX;
			OffsetY = offsetY;
			UpdatePointOnRegion();
			SubscribeEventsFromRegion();
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="pointOnRegion">IPointOnRegion</param>
		public PointOnRegion(IPointOnRegion pointOnRegion)
			: this(pointOnRegion.Region, pointOnRegion.OffsetX, pointOnRegion.OffsetY)
		{
		}

		#endregion

		#region Properties

		#region Properties IPointOnRegion

		/// <summary>
		/// Region
		/// </summary>
		public IRegion3D Region
		{
			get
			{
				return region;
			}

			set
			{
				if (region != value)
				{
					UnsubscribeEventsFromRegion();
					region = value;
					SubscribeEventsFromRegion();
				}
			}
		}

		/// <summary>
		/// offset of LocalX
		/// </summary>
		public double OffsetX
		{
			get;
			set;
		}

		/// <summary>
		/// offset of LocalY
		/// </summary>
		public double OffsetY
		{
			get;
			set;
		}

		#endregion

		#region IPoint3D Properties

		/// <summary>
		/// Coordinate X
		/// </summary>
		public double X
		{
			get;
			set;
		}

		/// <summary>
		/// Coordinate Y
		/// </summary>
		public double Y
		{
			get;
			set;
		}

		/// <summary>
		/// Coordinate Z
		/// </summary>
		public double Z
		{
			get;
			set;
		}

		#endregion

		#endregion

		#region Methods

		
		#region PointOnRegion Members

		protected void SubscribeEventsFromRegion()
		{
			//ElementBase elementRegion = region as ElementBase;
			//if (elementRegion != null)
			//{
			//  //elementRegion.ObjectChanged -= new ObjectChangedEventHandler(OnRegionChanged);
			//  //elementRegion.ObjectChanged += new ObjectChangedEventHandler(OnRegionChanged);
			//  PropertyChangedEventManager.AddListener(elementRegion, this);
			//}
		}

		protected void UnsubscribeEventsFromRegion()
		{
			//ElementBase elementRegion = region as ElementBase;
			//if (elementRegion != null)
			//{
			//  //elementRegion.ObjectChanged -= new ObjectChangedEventHandler(OnRegionChanged);
			//  PropertyChangedEventManager.RemoveListener(elementRegion, this);
			//}
		}

		protected void OnRegionChanged(object sender)
		{
			UpdatePointOnRegion();
			NotifyAllChanges();
		}

		protected override bool ReceiveWeakEventTo(Type managerType, object sender, EventArgs e)
		{
			/*base.ReceiveWeakEventTo(managerType, sender, e);
			if (managerType == typeof(PropertyChangedEventManager))
			{
				OnRegionChanged(sender);
				return true;
			}*/

			return false;
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Update Point on region
		/// </summary>
		private void UpdatePointOnRegion()
		{
			IPoint3D point = GetPointOnRegion();
			X = point.X;
			Y = point.Y;
			Z = point.Z;
		}

		/// <summary>
		/// Calculate Points On Region.
		/// </summary>
		/// <returns>returns the point on region</returns>
		private IPoint3D GetPointOnRegion()
		{
			IPoint3D pointOnRegion = new Point3D(OffsetX, OffsetY, 0.0);
			IMatrix44 matrix = GeomOperation.GetMatrixPlane(region);
			return matrix.TransformToGCS(pointOnRegion);
		}

		#endregion

		#endregion
	}
}