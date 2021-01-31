using System.Windows.Media.Media3D;

namespace CI.Common
{
	/// <summary>
	/// Defines an axis-aligned box-shaped 3D volume. 
	/// </summary>
	public class BoundingBox3D
	{
		#region Fields
		private double minX;
		private double maxX;
		private double minY;
		private double maxY;
		private double minZ;
		private double maxZ;
		#endregion

		#region Constructors
		/// <summary>
		/// Default constructor.
		/// </summary>
		public BoundingBox3D()
		{
			Reset();
		}

		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="source">Source object</param>
		public BoundingBox3D(BoundingBox3D source)
		{
			this.minX = source.minX;
			this.maxX = source.maxX;

			this.minY = source.minY;
			this.maxY = source.maxY;

			this.minZ = source.minZ;
			this.maxZ = source.maxZ;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets minimal x coordinate
		/// </summary>
		public double MinX
		{
			get { return minX; }
			set { minX = value; }
		}

		/// <summary>
		/// Gets or sets maximal x coordinate
		/// </summary>
		public double MaxX
		{
			get { return maxX; }
			set { maxX = value; }
		}

		/// <summary>
		/// Gets or sets minimal y coordinate
		/// </summary>
		public double MinY
		{
			get { return minY; }
			set { minY = value; }
		}

		/// <summary>
		/// Gets or sets maximal y coordinate
		/// </summary>
		public double MaxY
		{
			get { return maxY; }
			set { maxY = value; }
		}

		/// <summary>
		/// Gets or sets minimal z coordinate
		/// </summary>
		public double MinZ
		{
			get { return minZ; }
			set { minZ = value; }
		}

		/// <summary>
		/// Gets or sets maximal z coordinate
		/// </summary>
		public double MaxZ
		{
			get { return maxZ; }
			set { maxZ = value; }
		}

		/// <summary>
		/// Gets delta X
		/// </summary>
		public double DeltaX
		{
			get { return maxX - minX; }
		}

		/// <summary>
		/// Gets delta Y
		/// </summary>
		public double DeltaY
		{
			get { return maxY - minY; }
		}

		/// <summary>
		/// Gets delat Z
		/// </summary>
		public double DeltaZ
		{
			get { return maxZ - minZ; }
		}

		#endregion

		#region Methods
		/// <summary>
		/// Resets boundaries
		/// </summary>
		public void Reset()
		{
			this.minX = double.MaxValue;
			this.maxX = double.MinValue;

			this.minY = double.MaxValue;
			this.maxY = double.MinValue;

			this.minZ = double.MaxValue;
			this.maxZ = double.MinValue;
		}

		/// <summary>
		/// Inflates bounding box by new point.
		/// </summary>
		/// <param name="x">Coordinate x</param>
		/// <param name="y">Coordinate y</param>
		/// <param name="z">Coordinate z</param>
		public void Inflate(double x, double y, double z)
		{
			if (this.maxX < x)
			{
				this.maxX = x;
			}

			if (this.maxY < y)
			{
				this.maxY = y;
			}

			if (this.maxZ < z)
			{
				this.maxZ = z;
			}

			if (this.minX > x)
			{
				this.minX = x;
			}

			if (this.minY > y)
			{
				this.minY = y;
			}

			if (this.minZ > z)
			{
				this.minZ = z;
			}
		}

		/// <summary>
		/// Inflates bounding box by new point.
		/// </summary>
		/// <param name="point">New point</param>
		public void Inflate(ref Point3D point)
		{
			if (this.maxX < point.X)
			{
				this.maxX = point.X;
			}

			if (this.maxY < point.Y)
			{
				this.maxY = point.Y;
			}

			if (this.maxZ < point.Z)
			{
				this.maxZ = point.Z;
			}

			if (this.minX > point.X)
			{
				this.minX = point.X;
			}

			if (this.minY > point.Y)
			{
				this.minY = point.Y;
			}

			if (this.minZ > point.Z)
			{
				this.minZ = point.Z;
			}
		}

		/// <summary>
		/// Inflates bounding box by new point.
		/// </summary>
		/// <param name="point">New point</param>
		public void Inflate(Geometry3D.IPoint3D point)
		{
			if (this.maxX < point.X)
			{
				this.maxX = point.X;
			}

			if (this.maxY < point.Y)
			{
				this.maxY = point.Y;
			}

			if (this.maxZ < point.Z)
			{
				this.maxZ = point.Z;
			}

			if (this.minX > point.X)
			{
				this.minX = point.X;
			}

			if (this.minY > point.Y)
			{
				this.minY = point.Y;
			}

			if (this.minZ > point.Z)
			{
				this.minZ = point.Z;
			}
		}

		/// <summary>
		/// Inflates bounding box by another bounding box.
		/// </summary>
		/// <param name="boundingBox">Another bounding box</param>
		public void Inflate(BoundingBox3D boundingBox)
		{
			if (this.maxX < boundingBox.MaxX)
			{
				this.maxX = boundingBox.MaxX;
			}

			if (this.maxY < boundingBox.MaxY)
			{
				this.maxY = boundingBox.MaxY;
			}

			if (this.maxZ < boundingBox.MaxZ)
			{
				this.maxZ = boundingBox.MaxZ;
			}

			if (this.minX > boundingBox.MinX)
			{
				this.minX = boundingBox.MinX;
			}

			if (this.minY > boundingBox.MinY)
			{
				this.minY = boundingBox.MinY;
			}

			if (this.minZ > boundingBox.MinZ)
			{
				this.minZ = boundingBox.MinZ;
			}
		}

		/// <summary>
		/// Returns true if <paramref name="testedPoint"/> is inside this bounding box
		/// </summary>
		/// <param name="testedPoint">Tested point</param>
		/// <param name="boundingBoxOffset">Tolerance - the bounding box offset for testing</param>
		/// <returns>Returns true if point is located inside the bounding box</returns>
		public bool IsInside(ref Point3D testedPoint, double boundingBoxOffset = 1e-10)
		{
			if (testedPoint.X.IsGreaterOrEqual(this.minX, boundingBoxOffset) && testedPoint.X.IsLesserOrEqual(this.maxX, boundingBoxOffset) &&
				testedPoint.Y.IsGreaterOrEqual(this.minY, boundingBoxOffset) && testedPoint.Y.IsLesserOrEqual(this.maxY, boundingBoxOffset) &&
				testedPoint.Z.IsGreaterOrEqual(this.minZ, boundingBoxOffset) && testedPoint.Z.IsLesserOrEqual(this.maxZ, boundingBoxOffset))
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Returns true if <paramref name="testedPoint"/> is inside this bounding box
		/// </summary>
		/// <param name="testedPoint">Tested point</param>
		/// <param name="boundingBoxOffset">Tolerance - the bounding box offset for testing</param>
		/// <returns>Returns true if point is located inside the bounding box</returns>
		public bool IsPointInside(Point3D testedPoint, double boundingBoxOffset = 1e-10)
		{
			if (testedPoint.X.IsGreaterOrEqual(this.minX, boundingBoxOffset) && testedPoint.X.IsLesserOrEqual(this.maxX, boundingBoxOffset) &&
				testedPoint.Y.IsGreaterOrEqual(this.minY, boundingBoxOffset) && testedPoint.Y.IsLesserOrEqual(this.maxY, boundingBoxOffset) &&
				testedPoint.Z.IsGreaterOrEqual(this.minZ, boundingBoxOffset) && testedPoint.Z.IsLesserOrEqual(this.maxZ, boundingBoxOffset))
			{
				return true;
			}

			return false;
		}
		#endregion
	}
}
