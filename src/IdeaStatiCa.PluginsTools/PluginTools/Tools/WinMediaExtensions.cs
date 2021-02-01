using System;
using CI.Mathematics;
using WM = System.Windows.Media.Media3D;

namespace CI.Geometry3D
{
	/// <summary>
	/// Geometry operations related with Vector3D
	/// </summary>
	public static partial class GeomOperation
	{
		/// <summary>
		/// Adds two IPoint3D and returns the result as a Vector3D structure. 
		/// </summary>
		/// <param name="p1">The first point to add.</param>
		/// <param name="p2">The second point to add.</param>
		/// <returns>The sum of <paramref name="p1"/> and <paramref name="p2"/>.</returns>
		public static WM.Vector3D AddWM(IPoint3D p1, IPoint3D p2)
		{
			return new WM.Vector3D(p1.X + p2.X, p1.Y + p2.Y, p1.Z + p2.Z);
		}

		/// <summary>
		/// Subtracts <paramref name="p2"/> from <paramref name="p1"/>. 
		/// </summary>
		/// <param name="p1">The point to be subtracted from <paramref name="p2"/>.</param>
		/// <param name="p2">The point to subtract from <paramref name="p1"/>.</param>
		/// <returns>The result of subtracting <paramref name="p2"/> from <paramref name="p1"/>.</returns>
		public static WM.Vector3D SubWM(IPoint3D p1, IPoint3D p2)
		{
			return new WM.Vector3D(p1.X - p2.X, p1.Y - p2.Y, p1.Z- p2.Z);
		}

		/// <summary>
		/// Subtracts <paramref name="p2"/> from <paramref name="p1"/>. 
		/// </summary>
		/// <param name="p1">The point to be subtracted from <paramref name="p2"/>.</param>
		/// <param name="p2">The point to subtract from <paramref name="p1"/>.</param>
		/// <returns>The result of subtracting <paramref name="p2"/> from <paramref name="p1"/>.</returns>
		public static WM.Vector3D SubWM(WM.Point3D p1, IPoint3D p2)
		{
			return new WM.Vector3D(p1.X - p2.X, p1.Y - p2.Y, p1.Z - p2.Z);
		}

		/// <summary>
		/// Moves point by vector <paramref name="vect"/>
		/// </summary>
		/// <param name="src">Point</param>
		/// <param name="vect">Vector of shift</param>
		public static void Add(this IPoint3D src, ref WM.Vector3D vect)
		{
			src.X += vect.X;
			src.Y += vect.Y;
			src.Z += vect.Z;
		}

		/// <summary>
		/// Gets direction of the line segment (Normalized)
		/// </summary>
		/// <param name="src">Line segment</param>
		/// <returns>Direction</returns>
		public static WM.Vector3D GetDirection(this ILineSegment3D src)
		{
			var temp = new WM.Vector3D(src.EndPoint.X - src.StartPoint.X,
				src.EndPoint.Y - src.StartPoint.Y,
				src.EndPoint.Z - src.StartPoint.Z);

			temp.Normalize();
			return temp;
		}

		/// <summary>
		/// Gets direction of the begin and end point of the segment (normalized)
		/// </summary>
		/// <param name="src">Segment</param>
		/// <returns>Direction</returns>
		public static WM.Vector3D GetDirection(this ISegment3D src)
		{
			var temp = new WM.Vector3D(src.EndPoint.X - src.StartPoint.X,
				src.EndPoint.Y - src.StartPoint.Y,
				src.EndPoint.Z - src.StartPoint.Z);

			temp.Normalize();
			return temp;
		}

		/// <summary>
		/// Get StraightLine which connects start and end point of the <paramref name=""/>
		/// </summary>
		/// <param name="seg"></param>
		/// <returns>StraightLine</returns>
		public static StraightLine GetStraightLine(this ISegment3D seg)
		{
			return new StraightLine(seg.StartPoint, seg.EndPoint);
		}

		/// <summary>
		/// Converts to structure System.Windows.Media.Media3D.Point3D
		/// </summary>
		/// <param name="src"></param>
		/// <returns></returns>
		public static WM.Point3D ToMediaPoint(this IPoint3D src)
		{
			return new WM.Point3D(src.X, src.Y, src.Z);
		}

		/// <summary>
		/// Converts to structure System.Windows.Media.Media3D.Point3D
		/// </summary>
		/// <param name="src"></param>
		/// <returns></returns>
		public static WM.Point3D ToMediaPointXY0(this Geometry2D.IdaComPoint2D src)
		{
			return new WM.Point3D(src.X, src.Y, 0);
		}

		/// <summary>
		/// Converts to structure System.Windows.Media.Media3D.Vector3D
		/// </summary>
		/// <param name="src">Source vector</param>
		/// <returns>Structure System.Windows.Media.Media3D.Vector3D</returns>
		public static WM.Vector3D ToMediaVector(this Vector3D src)
		{
			return new WM.Vector3D(src.DirectionX, src.DirectionY, src.DirectionZ);
		}

		/// <summary>
		/// Converts to structure System.Windows.Media.Media3D.Point3D
		/// </summary>
		/// <param name="src">Source vector</param>
		/// <returns>Structure System.Windows.Media.Media3D.Point3D</returns>
		public static WM.Point3D ToMediaPoint(this Vector3D src)
		{
			return new WM.Point3D(src.DirectionX, src.DirectionY, src.DirectionZ);
		}

		/// <summary>
		/// Returns true is x or y or z is not a number
		/// </summary>
		/// <param name="src">Source vector</param>
		/// <returns>Returns true is x or y or z is not a number</returns>
		public static bool IsNaN(this Vector3D src)
		{
			return (double.IsNaN(src.DirectionX) || double.IsNaN(src.DirectionY) || double.IsNaN(src.DirectionZ));
		}

		/// <summary>
		/// Sets coordinates from <paramref name="src"/> to <paramref name="dest"/>
		/// </summary>
		/// <param name="src">Source</param>
		/// <param name="dest">Destination</param>
		public static void SetPoint(this WM.Point3D src, IPoint3D dest)
		{
			dest.X = src.X;
			dest.Y = src.Y;
			dest.Z = src.Z;
		}

        /// <summary>
        /// A
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="dirX"></param>
        /// <returns></returns>
        public static Matrix44 GetLCSMatrix(WM.Point3D origin, WM.Vector3D dirX)
		{
			dirX.Normalize();
			Vector3D localX = new Vector3D(dirX.X, dirX.Y, dirX.Z);
			Vector3D localZ = new Vector3D(0, 0, 1);
			if (GeomOperation.IsCollinear(localZ, localX, MathConstants.ZeroWeak)) //MathConstants.ZeroGeneral))
			{
				// When X is vertical then Z goes in X dir or -X
				//localZ = new Vector3D(0, 1, 0);
				if (localX.DirectionZ > 0)
				{
					localZ = new Vector3D(-1, 0, 0);
				}
				else
				{
					localZ = new Vector3D(1, 0, 0);
				}
			}

			//Vector3D localY = (localZ * localX).Normalize;
			//Matrix44 matrix = new Matrix44(origin, localX, localY);
			Vector3D localY = (localZ * localX);

			localZ = (localX * localY).Normalize;
			Matrix44 matrix = new Matrix44(origin, localX.Normalize, localY.Normalize, localZ);
			return matrix;
		}

		/// <summary>
		/// Evaluates line equation
		/// </summary>
		/// <param name="origin">The origin of the line</param>
		/// <param name="dir">The direction of the line</param>
		/// <param name="t">Parameter</param>
		/// <returns>Calculated point</returns>
		public static WM.Point3D LineEquation(ref WM.Point3D origin, ref WM.Vector3D dir, double t)
		{
			return new WM.Point3D(origin.X + t * dir.X, origin.Y + t * dir.Y, origin.Z + t * dir.Z);
		}

		/// <summary>
		/// Sets origin of the matrix
		/// </summary>
		/// <param name="src">Source matrix</param>
		/// <param name="origin">New origin</param>
		public static void SetOrigin(this Matrix44 src, WM.Point3D origin)
		{
			src.MatrixElements[0, 3] = origin.X;
			src.MatrixElements[1, 3] = origin.Y;
			src.MatrixElements[2, 3] = origin.Z;
		}

		/// <summary>
		/// Converts from CI.Geometry3D.Vector3D to System.Windows.Media.Media3D.Vector3D
		/// </summary>
		/// <param name="vect">Source</param>
		/// <returns>Vector</returns>
		public static CI.Geometry3D.Vector3D ToIndoVector3D(this System.Windows.Media.Media3D.Vector3D vect)
		{
			return new CI.Geometry3D.Vector3D(vect.X, vect.Y, vect.Z);
		}

		/// <summary>
		/// Converts from CI.Geometry3D.Point3D to System.Windows.Media.Media3D.Point3D
		/// </summary>
		/// <param name="pt">Source</param>
		/// <returns>Point</returns>
		public static CI.Geometry3D.IPoint3D ToIndoPoint3D(this System.Windows.Media.Media3D.Point3D pt)
		{
			return new CI.Geometry3D.Point3D(pt.X, pt.Y, pt.Z);
		}

		/// <summary>
		/// Returns true if X or Y or Z coordinate is NaN
		/// </summary>
		/// <param name="pt">Point</param>
		/// <returns>true if X or Y or Z coordinate is NaN</returns>
		public static bool IsNaN(this System.Windows.Media.Media3D.Point3D pt)
		{
			return double.IsNaN(pt.X) || double.IsNaN(pt.Y) || double.IsNaN(pt.Z);
		}

		/// <summary>
		/// Returns true if X and Y and Z coordinate are 0
		/// </summary>
		/// <param name="pt">Point</param>
		/// <param name="tolerance">Tolerance</param>
		/// <returns>true if X and Y and Z  coordinate are 0</returns>
		public static bool IsZero(this System.Windows.Media.Media3D.Point3D pt, double tolerance = 1e-9)
		{
			return pt.X.IsZero(tolerance) && pt.Y.IsZero(tolerance) && pt.Z.IsZero(tolerance);
		}

		/// <summary>
		/// Returns true if X or Y or Z coordinate is NaN
		/// </summary>
		/// <param name="vect">Vector</param>
		/// <returns>true if X or Y or Z coordinate is NaN</returns>
		public static bool IsNaN(this System.Windows.Media.Media3D.Vector3D vect)
		{
			return double.IsNaN(vect.X) || double.IsNaN(vect.Y) || double.IsNaN(vect.Z);
		}

		/// <summary>
		/// A
		/// </summary>
		/// <param name="vect"></param>
		/// <param name="testedVect"></param>
		/// <param name="tolerance"></param>
		/// <returns></returns>
		public static bool IsParallel(this System.Windows.Media.Media3D.Vector3D vect, System.Windows.Media.Media3D.Vector3D testedVect, double tolerance = 1e-8)
		{
			vect.Normalize();
			testedVect.Normalize();
			double dot = Math.Abs(WM.Vector3D.DotProduct(vect, testedVect));
			return dot.IsEqual(1, tolerance);
		}

		/// <summary>
		/// IsEqual - Determines whether leftValue and rightValue are equal.
		/// </summary>
		/// <param name="leftValue">Value on LHS of operator</param>
		/// <param name="rightValue">Value on RHS of operator</param>
		/// <param name="tolerance">Tolerance level for comparison</param>
		/// <returns>Return true if leftValue and rightValue are equal. Return false otherwise</returns>
		public static bool IsEqual(this System.Windows.Media.Media3D.Vector3D left, System.Windows.Media.Media3D.Vector3D right, double tolerance = 1e-8)
		{
			return left.X.IsEqual(right.X, tolerance) && left.Y.IsEqual(right.Y, tolerance) && left.Z.IsEqual(right.Z, tolerance);
		}

		/// <summary>
		/// IsEqual - Determines whether leftValue and rightValue are equal.
		/// </summary>
		/// <param name="leftValue">Value on LHS of operator</param>
		/// <param name="rightValue">Value on RHS of operator</param>
		/// <param name="tolerance">Tolerance level for comparison</param>
		/// <returns>Return true if leftValue and rightValue are equal. Return false otherwise</returns>
		public static bool IsEqual(this System.Windows.Media.Media3D.Point3D left, System.Windows.Media.Media3D.Point3D right, double tolerance = 1e-8)
		{
			return left.X.IsEqual(right.X, tolerance) && left.Y.IsEqual(right.Y, tolerance) && left.Z.IsEqual(right.Z, tolerance);
		}

		public static void Move(ILineSegment3D segment, WM.Vector3D displacement)
		{
			segment.StartPoint.Add(ref displacement);
			segment.EndPoint.Add(ref displacement);
		}

		/// <summary>
		/// Gets relative position of <paramref name="testedPt"/> relatively to the oriented line <paramref name="start"/> - <paramref name="end"/>
		/// </summary>
		/// <param name="start">Start point of the oriented line</param>
		/// <param name="end">End point of the oriented line</param>
		/// <param name="testedPt">Tested point</param>
		/// <returns>Relative position of <paramref name="testedPt"/></returns>
		public static double GetRelativePos(ref WM.Point3D start, ref WM.Point3D end, ref WM.Point3D testedPt)
		{
			var origVect = end - start;
			double origVectLen = origVect.Length;
			var posVect = testedPt - start;
			double posVectLen = origVect.Length;

			if (origVectLen.IsZero())
			{
				if(posVectLen.IsZero())
				{
					return 0;
				}
				else
				{
					return double.NaN;
				}
			}

			var weldDir = origVect;
			weldDir.Normalize();
			double vectDot = WM.Vector3D.DotProduct(posVect, weldDir);
			if (vectDot.IsEqual(origVectLen))
			{
				return 1;
			}

			double relPos = (vectDot / origVectLen);
			return relPos;
		}

		/// <summary>
		/// Creates Point3D from Point in plane xy
		/// </summary>
		/// <param name="src">Source point</param>
		/// <returns>Point3D</returns>
		public static WM.Point3D To3D_XY(this System.Windows.Point src)
		{
			return new WM.Point3D(src.X, src.Y, 0);
		}

		/// <summary>
		/// Returns point wich is located in the middle of <paramref name="p1"/> and <paramref name="p1"/>
		/// </summary>
		/// <param name="p1">First point</param>
		/// <param name="p2">Second point</param>
		/// <returns></returns>
		public static WM.Point3D GetMidPoint(ref WM.Point3D p1, ref WM.Point3D p2)
		{
			return new WM.Point3D(0.5 * (p1.X + p2.X), 0.5 * (p1.Y + p2.Y), 0.5 * (p1.Z + p2.Z));
		}
	}
}
