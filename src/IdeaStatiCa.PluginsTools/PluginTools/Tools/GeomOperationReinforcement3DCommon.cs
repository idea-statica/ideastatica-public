using System;

namespace CI.Geometry3D
{
	/// <summary>
	/// Geometry operations related with Reinforcement3D
	/// </summary>
	public static partial class GeomOperation
	{

		// IMPORTANT !!!
		// copy file in "...\_Sources\Modelers\Common\IdeaStatiCa.PluginTools\Tools\" because reference missing

		#region GeomOperation


		/// <summary>
		/// Checks if points position is equal
		/// </summary>
		/// <param name="pt1">first point</param>
		/// <param name="pt2">second point</param>
		/// <returns>true: points position is equal</returns>
		public static bool IsPointEqual(IPoint3D pt1, IPoint3D pt2)
		{
			if (!(Math.Abs(pt1.X - pt2.X).IsZero()))
			{
				return false;
			}
			if (!(Math.Abs(pt1.Y - pt2.Y).IsZero()))
			{
				return false;
			}
			if (!(Math.Abs(pt1.Z - pt2.Z).IsZero()))
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Checks if points position is equal
		/// </summary>
		/// <param name="pt1">first point</param>
		/// <param name="pt2">second point</param>
		/// <param name="pointMergeTolerance">tolerance</param>
		/// <returns>true: points position is equal</returns>
		public static bool IsPointEqual(IPoint3D pt1, IPoint3D pt2, double pointMergeTolerance)
		{
			if (!(Math.Abs(pt1.X - pt2.X).IsZero(pointMergeTolerance)))
			{
				return false;
			}
			if (!(Math.Abs(pt1.Y - pt2.Y).IsZero(pointMergeTolerance)))
			{
				return false;
			}
			if (!(Math.Abs(pt1.Z - pt2.Z).IsZero(pointMergeTolerance)))
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// checks if vectors are parallel
		/// </summary>
		/// <param name="vec1">first vector</param>
		/// <param name="vec2">second vector</param>
		/// <returns>true: if vectors are parallel</returns>
		public static bool IsVectorsParallel(Vector3D vec1, Vector3D vec2)
		{
			// Two vectors are perpendicular if their dot product is zero
			// and parallel if their dot product is 1.
			double d1 = vec1 | vec2;
			if ((Math.Abs(Math.Abs(d1) - 1.0)).IsZero())
			{
				return true;
			}
			return false;
		}


		// use : GeomOperation.Subtract
		//public static Vector3D GetVector3D(IPoint3D point1, IPoint3D point2)
		//{
		//	// point2 - point1
		//	var vec1 = new Vector3D(point2.X - point1.X, point2.Y - point1.Y, point2.Z - point1.Z);
		//	return vec1.Normalize;
		//}

		/// <summary>
		/// Multiply Point3D By Vector3D
		/// </summary>
		/// <param name="source">point</param>
		/// <param name="direction">direction</param>
		/// <param name="length">length</param>
		/// <param name="normalizeVector">normalize Vector</param>
		public static void MultiplyPoint3DByVector3D(IPoint3D source, Vector3D direction, double length, bool normalizeVector )
		{
			// point = point0 + length * direction
			var vec1 = new Vector3D(direction);
			if (normalizeVector)
			{
				vec1 = vec1.Normalize;
			}
			//var retVal = new RebarPoint3D(point0);
			source.X += length * vec1.DirectionX;
			source.Y += length * vec1.DirectionY;
			source.Z += length * vec1.DirectionZ;
			//return retVal;
		}

		/// <summary>
		/// Add Point3D And Vector3D
		/// </summary>
		/// <param name="source">point</param>
		/// <param name="direction">vector</param>
		public static void AddPoint3DAndVector3D(IPoint3D source, Vector3D direction)
		{
			//var retVal = new RebarPoint3D(point0);
			source.X += direction.DirectionX;
			source.Y += direction.DirectionY;
			source.Z += direction.DirectionZ;
			//return retVal;
		}

		/// <summary>
		/// Add Point3D And Point3D
		/// </summary>
		/// <param name="source">point</param>
		/// <param name="addPoint">point</param>
		public static void AddPoint3DAndPoint3D(IPoint3D source, IPoint3D addPoint)
		{
			//var retVal = new RebarPoint3D(point0);
			source.X += addPoint.X;
			source.Y += addPoint.Y;
			source.Z += addPoint.Z;
			//return retVal;
		}


		/// <summary>
		/// Get normal created by three points
		/// </summary>
		/// <param name="point0">first point</param>
		/// <param name="pointX">point which lie on X-axis</param>
		/// <param name="pointYdir">point which defines direction of Y-axis</param>
		/// <returns>Normal created by three points</returns>
		public static Vector3D GetNormalByThreePoints(IPoint3D point0, IPoint3D pointX, IPoint3D pointYdir)
		{
			var vec1 = GeomOperation.Subtract(pointX, point0); // new Vector3D(pointX.X - point0.X, pointX.Y - point0.Y, pointX.Z - point0.Z);
			var vec2 = GeomOperation.Subtract(pointYdir, point0); // new Vector3D(pointYdir.X - point0.X, pointYdir.Y - point0.Y, pointYdir.Z - point0.Z);
			Vector3D norm = vec1 * vec2;
			return norm.Normalize;
		}

		/// <summary>
		/// Get transformation matrix by three points
		/// </summary>
		/// <param name="point0">first point</param>
		/// <param name="pointX">point which lie on X-axis</param>
		/// <param name="pointYdir">point which defines direction of Y-axis</param>
		/// <returns>Transformation matrix by three points</returns>
		public static IMatrix44 GetTransformationByThreePoints(IPoint3D point0, IPoint3D pointX, IPoint3D pointYdir)
		{
			var vec1 = GeomOperation.Subtract(pointX, point0); // new Vector3D(pointX.X - point0.X, pointX.Y - point0.Y, pointX.Z - point0.Z);
			var vec2 = GeomOperation.Subtract(pointYdir, point0); // new Vector3D(pointYdir.X - point0.X, pointYdir.Y - point0.Y, pointYdir.Z - point0.Z);
			var vecX = vec1.Normalize;
			Vector3D vecZ = (vecX * vec2).Normalize;
			Vector3D vecY = (vecZ * vecX).Normalize;
			return new Matrix44(point0, vecX, vecY, vecZ);
		}

		/// <summary>
		/// Get transformation matrix by three points
		/// </summary>
		/// <param name="point0">first point</param>
		/// <param name="xAxis">X-Axis</param>
		/// <param name="yDirection">vector which defines direction of Y-axis</param>
		/// <returns>Transformation matrix by three points</returns>
		public static IMatrix44 GetTransformationByThreePoints(IPoint3D point0, Vector3D xAxis, Vector3D yDirection)
		{
			var vec2 = yDirection.Normalize;
			var vecX = xAxis.Normalize;
			Vector3D vecZ = (vecX * vec2).Normalize;
			Vector3D vecY = (vecZ * vecX).Normalize;
			return new Matrix44(point0, vecX, vecY, vecZ);
		}

		/// <summary>
		/// Get transformation matrix by three points
		/// </summary>
		/// <param name="point0">first point</param>
		/// <param name="xAxis">X-Axis</param>
		/// <param name="pointYdir">point which defines direction of Y-axis</param>
		/// <returns>Transformation matrix by three points</returns>
		public static IMatrix44 GetTransformationByThreePoints(IPoint3D point0, Vector3D xAxis, IPoint3D pointYdir)
		{
			var vec2 = GeomOperation.Subtract(pointYdir, point0); // new Vector3D(pointYdir.X - point0.X, pointYdir.Y - point0.Y, pointYdir.Z - point0.Z);
			var vecX = xAxis.Normalize;
			Vector3D vecZ = (vecX * vec2).Normalize;
			Vector3D vecY = (vecZ * vecX).Normalize;
			return new Matrix44(point0, vecX, vecY, vecZ);
		}

		/// <summary>
		/// Transform by act on left
		/// </summary>
		/// <param name="t"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <param name="w"></param>
		/// <returns></returns>
		public static double[] Matrix44ActOnLeft(IMatrix44 t,
			double x, double y, double z, double w)
		{
			var matrix = (t as Matrix44).MatrixElements;
			return new double[]
			{
				matrix[0, 0] * x + matrix[0, 1] * y + matrix[0, 2] * z + matrix[0, 3] * w,
				matrix[1, 0] * x + matrix[1, 1] * y + matrix[1, 2] * z + matrix[1, 3] * w,
				matrix[2, 0] * x + matrix[2, 1] * y + matrix[2, 2] * z + matrix[2, 3] * w,
				matrix[3, 0] * x + matrix[3, 1] * y + matrix[3, 2] * z + matrix[3, 3] * w
			};
		}

		/// <summary>
		/// Transform by act on left (w=1.0)
		/// </summary>
		/// <param name="t"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <returns></returns>
		public static double[] Matrix44ActOnLeftOne(IMatrix44 t,
			double x, double y, double z)
		{
			var matrix = (t as Matrix44).MatrixElements;
			return new double[]
			{
				matrix[0, 0] * x + matrix[0, 1] * y + matrix[0, 2] * z + matrix[0, 3],
				matrix[1, 0] * x + matrix[1, 1] * y + matrix[1, 2] * z + matrix[1, 3],
				matrix[2, 0] * x + matrix[2, 1] * y + matrix[2, 2] * z + matrix[2, 3]
			};
		}


		/// <summary>
		/// Transform by act on left (w=0.0)
		/// </summary>
		/// <param name="t"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <returns></returns>
		public static double[] Matrix44ActOnLeftZero(IMatrix44 t,
			double x, double y, double z)
		{
			var matrix = (t as Matrix44).MatrixElements;
			return new double[]
			{
				matrix[0, 0] * x + matrix[0, 1] * y + matrix[0, 2] * z,
				matrix[1, 0] * x + matrix[1, 1] * y + matrix[1, 2] * z,
				matrix[2, 0] * x + matrix[2, 1] * y + matrix[2, 2] * z
			};
		}

		/// <summary>
		/// Create orientation transformation matrix which pass throu point and is pointing to direction
		/// </summary>
		/// <param name="position">point</param>
		/// <param name="direction">direction </param>
		/// <returns></returns>
		public static IMatrix44 GetOrientationTransformation(IPoint3D position, Vector3D direction)
		{
			var retVal = new Matrix44();
			retVal.SetToIdentity();
			var vector3D = direction.Normalize;

			double angleInXY = Math.Atan2(vector3D.DirectionY, vector3D.DirectionX);
			double d1 = Math.Sqrt(vector3D.DirectionX * vector3D.DirectionX + vector3D.DirectionY * vector3D.DirectionY);
			double angleFromXY = Math.Atan2(vector3D.DirectionZ, d1);

			var tr1 = new Matrix44();
			tr1.SetToTranslation(new Vector3D(position.X, position.Y, position.Z));
			var rot1 = new Matrix44();
			rot1.SetToRotation(-angleInXY, Axis.ZAxis); // (angleInXY, Axis.ZAxis);
			var rot2 = new Matrix44();
			rot2.SetToRotation(angleFromXY, Axis.YAxis); // (-angleFromXY, Axis.YAxis);

			var retVal1 = Matrix44.Multiply(tr1, rot1);
			retVal = Matrix44.Multiply(retVal1, rot2);

			return retVal;
		}

		/// <summary>
		/// Swap XYZ to ZYX
		/// </summary>
		/// <param name="XYZCSys">transformation matrix</param>
		/// <returns></returns>
		public static IMatrix44 SwapXYZtoZYX(IMatrix44 XYZCSys)
		{
			var retVal = new Matrix44();
			retVal.Origin = new Point3D(XYZCSys.Origin);
			retVal.AxisX = new Vector3D(XYZCSys.AxisZ);
			retVal.AxisY = new Vector3D(XYZCSys.AxisY);
			var vec1 = retVal.AxisX * retVal.AxisY;
			retVal.AxisZ = vec1.Normalize;
			return retVal;
		}


		/// <summary>
		/// rotate point around vector in anticlockwise
		/// </summary>
		/// <param name="point">origin</param>
		/// <param name="basePoint">rotation base point</param>
		/// <param name="normalVector">roation around vector</param>
		/// <param name="angleDeg">angle of rotation</param>
		/// <returns>new point</returns>
		public static IPoint3D RotatePoint3DAroundVector(IPoint3D point, IPoint3D basePoint, Vector3D normalVector, double angleDeg)
		{
			// support basePoint = null

			if (null == point)
			{
				return null;
			}

			var retVal = new Point3D(point);

			if (angleDeg.IsZero())
			{
				return retVal;
			}

			double angleRad = angleDeg * Math.PI / 180.0;

			if (null != basePoint)
			{
				retVal.X -= basePoint.X;
				retVal.Y -= basePoint.Y;
				retVal.Z -= basePoint.Z;
			}

			// rotate
			Matrix44 mat = new Matrix44();
			var n = normalVector.Normalize;
			mat.Rotate(angleRad, n);
			var retPoint = mat.TransformToGCS(retVal);
			retVal.X = retPoint.X;
			retVal.Y = retPoint.Y;
			retVal.Z = retPoint.Z;

			// move origin back to insertPoint
			if (null != basePoint)
			{
				retVal.X += basePoint.X;
				retVal.Y += basePoint.Y;
				retVal.Z += basePoint.Z;
			}

			return retVal;
		}

		/// <summary>
		/// Translate Point3D
		/// </summary>
		/// <param name="point">point</param>
		/// <param name="distance">distance</param>
		public static void TranslatePoint3D(IPoint3D point, Vector3D distance)
		{
			point.X += distance.DirectionX;
			point.Y += distance.DirectionY;
			point.Z += distance.DirectionZ;
		}

		/// <summary>
		/// Translate Point3D
		/// </summary>
		/// <param name="point">point</param>
		/// <param name="direction">direction</param>
		/// <param name="distance">distance</param>
		public static void TranslatePoint3D(IPoint3D point, Vector3D direction, double distance)
		{
			point.X += direction.DirectionX * distance;
			point.Y += direction.DirectionY * distance;
			point.Z += direction.DirectionZ * distance;
		}


		/// <summary>
		/// Perpendicular Point To Line
		/// </summary>
		/// <param name="fromPoint">point</param>
		/// <param name="linePoint1">start point of the line</param>
		/// <param name="linePoint2">end point of the line</param>
		/// <returns>perpendicular point</returns>
		public static IPoint3D PerpendicularPointToLine(IPoint3D fromPoint, IPoint3D linePoint1, IPoint3D linePoint2)
		{
			// support basePoint = null

			if ((null == fromPoint) | (null == linePoint1) | (null == linePoint2))
			{
				return null;
			}

			var v = new Vector3D((linePoint2.X - linePoint1.X), (linePoint2.Y - linePoint1.Y), (linePoint2.Z - linePoint1.Z));
			var w = new Vector3D((fromPoint.X - linePoint1.X), (fromPoint.Y - linePoint1.Y), (fromPoint.Z - linePoint1.Z));

			//v.Normalize()
			//w.Normalize()

			double c1 = w | v; // dot
			double c2 = v | v; // dot

			//If c2 = 0 Then Return 0
			double b = c1 / c2;
			var retVal = new Point3D(linePoint1.X + b * v.DirectionX, linePoint1.Y + b * v.DirectionY, linePoint1.Z + b * v.DirectionZ);

			return retVal;
		}

		/// <summary>
		/// Get distance between two points
		/// </summary>
		/// <param name="pt1">first point</param>
		/// <param name="pt2">second point</param>
		/// <returns>Distance</returns>
		public static double GetDistance(IPoint3D pt1, IPoint3D pt2)
		{
			double d1 = Math.Pow(pt2.X - pt1.X, 2.0) +
				Math.Pow(pt2.Y - pt1.Y, 2.0) +
				Math.Pow(pt2.Z - pt1.Z, 2.0);
			return Math.Sqrt(d1);
		}


		#endregion GeomOperation

	}
}