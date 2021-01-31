using CI.DataModel;
using System;

namespace CI.Geometry3D
{
	public class CoordSystemByVector : CoordSystem, ICoordSystemByVector
	{
		#region Fields
		private Vector3D vecX;
		private Vector3D vecY;
		private Vector3D vecZ;

		[NonSerialized]
		private Matrix44 transformation;
		#endregion

		#region Constructors

		/// <summary>
		/// Empty constructor
		/// </summary>
		public CoordSystemByVector()
		{
			vecX = new Vector3D(1.0, 0.0, 0.0);
			vecY = new Vector3D(0.0, 1.0, 0.0);
			vecZ = new Vector3D(0.0, 0.0, 1.0);
		}

		/// <summary>
		/// Constructor with vectors
		/// </summary>
		/// <param name="vecX">axis X vector</param>
		/// <param name="vecY">axis Y vector</param>
		/// <param name="vecZ">axis Z vector</param>
		public CoordSystemByVector(Vector3D vecX, Vector3D vecY, Vector3D vecZ)
		{
			this.vecX = vecX;
			this.vecY = vecY;
			this.vecZ = vecZ;
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="source">source</param>
		public CoordSystemByVector(ICoordSystemByVector source)
		{
			vecX = source.VecX;
			vecY = source.VecY;
			vecZ = source.VecZ;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Axis X unit vector
		/// </summary>
		public Vector3D VecX
		{
			get
			{
				return vecX;
			}

			set
			{
				vecX = value;
				Reset();
			}
		}

		/// <summary>
		///  Axis Y unit vector
		/// </summary>
		public Vector3D VecY
		{
			get
			{
				return vecY;
			}

			set
			{
				vecY = value;
				Reset();
			}
		}

		/// <summary>
		///  Axis Z unit vector
		/// </summary>
		public Vector3D VecZ
		{
			get
			{
				return vecZ;
			}

			set
			{
				vecZ = value;
				Reset();
			}
		}

		/// <summary>
		/// Get LCS based on axis vectors
		/// </summary>
		/// <param name="originPoint">Origin point of returned LCS</param>
		/// <returns>Matrix44 as LCS</returns>
		public IMatrix44 GetCoordinateSystemMatrix(IPoint3D originPoint)
		{
			if (transformation == null)
			{
				transformation = new Matrix44(originPoint, VecX, VecY, VecZ);
			}
			else
			{
				transformation.Origin = originPoint;
			}

			return transformation;
		}

		/// <summary>
		/// Get LCS based on inner properties
		/// </summary>
		/// <param name="originPoint">Origin point of returned LCS</param>
		/// <param name="pointAxisX">Point on Axis X</param>
		/// <returns>Matrix44 as LCS</returns>
		public override IMatrix44 GetCoordinateSystemMatrix(IPoint3D originPoint, IPoint3D pointAxisX)
		{
			if (transformation == null)
			{
				transformation = new Matrix44(originPoint, VecX, VecY, VecZ);
			}
			else
			{
				transformation.Origin = originPoint;
			}

			return transformation;
		}

		/// <summary>
		/// Copies data
		/// </summary>
		/// <param name="to">it to is null, new object is created</param>
		public void CopyTo(ref ICoordSystemByVector to)
		{
			if ((to == null) || !(to is ICoordSystemByVector))
			{
				to = new CoordSystemByVector(vecX, vecY, vecZ);
			}
			else
			{
				to.VecX = vecX;
				to.VecY = vecY;
				to.VecZ = vecZ;
			}
		}

		/// <summary>
		/// Copies data
		/// </summary>
		/// <param name="to">it to is null, new object is created</param>
		public override void CopyTo(ref ICoordSystem to)
		{
			ICoordSystemByVector tov = to as ICoordSystemByVector;
			CopyTo(ref tov);
			to = tov as ICoordSystem;
		}

		/// <summary>
		/// Reset
		/// </summary>
		private void Reset()
		{
			transformation = null;
		}

		#endregion
	}
}