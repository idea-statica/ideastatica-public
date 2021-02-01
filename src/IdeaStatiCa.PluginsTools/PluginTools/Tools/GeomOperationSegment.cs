using CI.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CI.Geometry3D
{
	/// <summary>
	/// Geometry operations related with Segment3D
	/// </summary>
	public static partial class GeomOperation
	{
		/// <summary>
		/// Gets verticall flag
		/// </summary>
		/// <param name="segment">Geometrical segment3D</param>
		/// <returns>true if ILineSegment3D is vertical</returns>
		public static bool IsVerticall(ISegment3D segment)
		{
			if (segment.GetType() == typeof(LineSegment3D))
			{
				IPoint3D start = segment.StartPoint;
				IPoint3D end = segment.EndPoint;

				Vector3D localX = GeomOperation.Subtract(end, start).Normalize;
				Vector3D localZ = new Vector3D(0, 0, 1);

				// Fixed to 1e-3 - don't change because of AxisVM #11436
				if (GeomOperation.IsCollinear(localZ, localX, 3e-3))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Creates segments from circle
		/// </summary>
		/// <param name="pointCurve">Begin point of circle</param>
		/// <param name="centre">Centre point of circle</param>
		/// <param name="endPointCurve">End point of circle</param>
		/// <param name="angle">Arc angle</param>
		/// <param name="normalVector">Normal vector</param>
		/// <returns>List of ArcSegments</returns>
		public static IList<ISegment3D> CreateSegmentsFromCircle(IPoint3D pointCurve, IPoint3D centre, IPoint3D endPointCurve, double angle, Vector3D normalVector)
		{
			double partAngle = System.Math.PI * 0.5;

			Vector3D localX = GeomOperation.Subtract(pointCurve, centre).Normalize;
			Vector3D localY = (normalVector * localX).Normalize;
			Matrix44 mat = new Matrix44(centre, localX, localY);

			//angle = System.Math.Abs(angle);
			double r = GeomOperation.Distance(pointCurve, centre);
			double dnum = System.Math.Abs(angle) / partAngle;
			int inum = (int)System.Math.Round(dnum);
			if (inum < 2)
			{
				inum = 2;
			}

			partAngle = angle / inum;

			IList<IPoint3D> arcPoints = new List<IPoint3D>();
			IPoint3D midPoint = new Point3D(r, 0, 0);

			arcPoints.Add(pointCurve);
			for (int i = 0; i < inum - 1; i++)
			{
				mat.Rotate(partAngle, normalVector);
				IPoint3D retPoint = mat.TransformToGCS(midPoint);
				arcPoints.Add(retPoint);
			}

			arcPoints.Add(endPointCurve);

			IList<ISegment3D> retSeg = new List<ISegment3D>();
			for (int i = 0; i < arcPoints.Count - 1; i += 2)
			{
				IArcSegment3D seg = new ArcSegment3D(arcPoints[i], arcPoints[i + 1], arcPoints[i + 2]);
				retSeg.Add(seg);
			}

			return retSeg;
		}

		/// <summary>
		/// Create a new arc with given displacement from original arc
		/// </summary>
		/// <param name="arc">Original arc</param>
		/// <param name="displacement">Displacement vector</param>
		/// <returns>Displaced arc</returns>
		public static IArcSegment3D Add(IArcSegment3D arc, Vector3D displacement)
		{
			IArcSegment3D newArc = new ArcSegment3D
			{
				StartPoint = Add(arc.StartPoint, displacement),
				IntermedPoint = Add(arc.IntermedPoint, displacement),
				EndPoint = Add(arc.EndPoint, displacement)
			};
			return newArc;
		}

		/// <summary>
		/// Create a new line with given displacement from original line
		/// </summary>
		/// <param name="line">Original line</param>
		/// <param name="displacement">Displacement vector</param>
		/// <returns>Displaced line</returns>
		public static ILineSegment3D Add(ILineSegment3D line, Vector3D displacement)
		{
			ILineSegment3D newLine = new LineSegment3D
			{
				StartPoint = Add(line.StartPoint, displacement),
				EndPoint = Add(line.EndPoint, displacement)
			};
			return newLine;
		}

		/// <summary>
		/// Divide Parabola into quadratic spline and calculate its relative position.
		/// </summary>
		/// <param name="segmentParabola">Parabola Segment</param>
		/// <param name="stepAngle">Step Angle</param>
		/// <param name="relativeCollection">Collection of relative position</param>
		/// <param name="minTiles">Min tiles</param>
		/// <returns>relative positon count of parabola</returns>
		public static int GetQuadraticSplineRelativePosition(IArcSegment3D segmentParabola, double stepAngle, ref IList<double> relativeCollection, int minTiles = 4)
		{
			Vector3D previousTangent = new Vector3D();
			double capacity = 0.0;
			double offset = 0.0;
			double relativePosition = 0.0;
			double MaximumShift = 1.0 / (double)minTiles; //0.25;
			double shift = MaximumShift;
			int count = 1;
			int maximumPoints = 4000;

			ParabolaSegment3D parabola = segmentParabola as ParabolaSegment3D;
			if (parabola == null)
			{
				throw new InvalidCastException("Wrong segment data");
			}

			while (maximumPoints-- > 1)
			{
				IPoint3D start = null;
				parabola.Property.GetPointOnParabola(segmentParabola, relativePosition, ref start);
				IPoint3D intermediate = null;
				parabola.Property.GetPointOnParabola(segmentParabola, relativePosition + MathConstants.ZeroWeak, ref intermediate);
				IPoint3D end = null;
				parabola.Property.GetPointOnParabola(segmentParabola, relativePosition + 2 * MathConstants.ZeroWeak, ref end);

				Vector3D curvature = (1 / (MathConstants.ZeroWeak * MathConstants.ZeroWeak)) * Subtract(Add(end, start), Multiply(intermediate, 2));
				Vector3D tangent = (1 / MathConstants.ZeroWeak) * Subtract(intermediate, start);

				double tolerance = ~tangent;
				if (tolerance.IsEqual(0))
				{
					shift = MaximumShift;
					relativePosition += shift;
					continue;
				}

				capacity = ~(tangent * curvature) / Math.Pow(tolerance, 3);
				offset = capacity * tolerance;

				if (relativePosition.IsGreater(0) && (maximumPoints > 0))
				{
					double realAngle = GeomOperation.GetAngle(tangent, previousTangent);
					if (realAngle > stepAngle * 1.1)
					{
						relativePosition -= shift;
						shift /= realAngle / stepAngle;
						relativePosition += shift;
						continue;
					}
				}

				if (relativePosition.IsGreater(1.0 - (0.1 * shift)))
				{
					break;
				}

				shift = MaximumShift;
				double maximumShift = MaximumShift;
				if (offset.IsGreater(MathConstants.ZeroMaximum))
				{
					maximumShift = stepAngle / offset;
				}

				if (maximumShift.IsLesser(shift) && (maximumPoints > 0))
				{
					shift = maximumShift;
				}

				relativeCollection.Add(relativePosition);
				count++;

				relativePosition += shift;
				previousTangent = tangent;
			}

			relativeCollection.Add(1);
			return count;
		}

		/// <summary>
		/// Make a clone of given segment
		/// </summary>
		/// <param name="segment">Original segment</param>
		/// <returns>New segment</returns>
		public static ISegment3D Clone(ISegment3D segment)
		{
			if (segment.SegmentType == SegmentType.Line)
			{
				ILineSegment3D line = segment as ILineSegment3D;
				if (line == null)
				{
					throw new InvalidCastException("Wrong segment data");
				}

				return new LineSegment3D(line);
			}
			else if (segment.SegmentType == SegmentType.CircularArc)
			{
				IArcSegment3D arc = segment as IArcSegment3D;
				if (arc == null)
				{
					throw new InvalidCastException("Wrong segment data");
				}

				return new ArcSegment3D(arc);
			}
			else if (segment.SegmentType == SegmentType.Parabola)
			{
				IArcSegment3D arc = segment as IArcSegment3D;
				if (arc == null)
				{
					throw new InvalidCastException("Wrong segment data");
				}

				return new ParabolaSegment3D(arc);
			}

			throw new NotSupportedException();
		}

		/// <summary>
		/// Calculate and return the arc angle in radians
		/// </summary>
		/// <param name="arc">Input arc segment</param>
		/// <returns>The arc angle (in radian)</returns>
		public static double GetArcAngle(IArcSegment3D arc)
		{
			double arcAngle = 0;
			if (arc is ArcSegment3D)
			{
				IPoint3D centrePoint = new Point3D();
				if (GetCentrePoint(arc, ref centrePoint))
				{
					Vector3D vectorX = Subtract(arc.StartPoint, centrePoint).Normalize;
					Vector3D vectorY = (GetNormal(arc, 1e-9) * vectorX).Normalize;
					Vector3D vector = Subtract(arc.EndPoint, centrePoint);

					double y = vector | vectorY;
					//double x = vector | vectorX;
					//arcAngle = Math.Atan2(y, x);
					//if (arcAngle.IsLesser(0, MathConstants.ZeroGeneral))
					//{
					//  arcAngle += 2 * Math.PI;
					//}

					arcAngle = GeomOperation.GetAngle(vectorX, vector);
					if (y.IsLesser(0))
					{
						arcAngle = 2 * Math.PI - arcAngle;
					}

					//Vector3D vectorX = Subtract(arc.StartPoint, centrePoint);
					//Vector3D vectorY = Subtract(arc.EndPoint, centrePoint);
					//arcAngle = GeomOperation.GetAngle(vectorX, vectorY);
				}
			}
			else if (arc is ParabolaSegment3D)
			{
				var par = arc as ParabolaSegment3D;
				arcAngle = par.GetAngleChange();
			}

			return arcAngle;
		}

		/// <summary>
		/// Calculate centre point of an arc
		/// </summary>
		/// <param name="arc">Arc for which center point is calculated</param>
		/// <param name="centrePoint">centre of the given arc</param>
		/// <param name="tolerance">tolerance for zero value</param>
		/// <returns>True if arc is valid</returns>
		public static bool GetCentrePoint(IArcSegment3D arc, ref IPoint3D centrePoint, double tolerance = MathConstants.ZeroMaximum)
		{
			Vector3D vectorIE = Subtract(arc.EndPoint, arc.IntermedPoint);
			double d = vectorIE | vectorIE;
			if (d.IsZero(tolerance))
			{
				return false;
			}

			Vector3D vectorBI = Subtract(arc.IntermedPoint, arc.StartPoint);
			Vector3D normalVector = (((vectorBI | vectorIE) / d) * vectorIE) - vectorBI;
			double d1 = 2 * vectorBI | normalVector;
			if (d1.IsZero(tolerance))
			{
				return false;
			}

			double relativeOffset = 0.0;
			if (tolerance == MathConstants.ZeroMaximum)
			{
				relativeOffset = vectorBI | (Subtract(arc.StartPoint, arc.EndPoint) / d1);
			}
			else
			{
				var v = Subtract(arc.StartPoint, arc.EndPoint);
				var v2 = new Vector3D(v.DirectionX / d1, v.DirectionY / d1, v.DirectionZ / d1);
				relativeOffset = vectorBI | v2;
			}

			IPoint3D circlePoint = Multiply(Add(arc.IntermedPoint, arc.EndPoint), 0.5);
			centrePoint = Add(circlePoint, (normalVector * relativeOffset));
			return true;
		}

		/// <summary>
		/// Calculate local transformation matrix on the segement at given point
		/// </summary>
		/// <param name="segment">Segment</param>
		/// <param name="relativePosition">Relative position on this segment (0-1)</param>
		/// <returns>Transformation matrix</returns>
		public static IMatrix44 GetTransformation(ISegment3D segment, double relativePosition)
		{
			Segment3D fullSegment = segment as Segment3D;

			// Origin point at position
			IPoint3D originPoint = new Point3D();
			GeomOperation.GetPointOnSegment(fullSegment, relativePosition, ref originPoint);

			// Point on Axis X - in tangent direction
			Vector3D tangentVector = new Vector3D();
			GetTangentOnSegment(segment, relativePosition, ref tangentVector);
			IPoint3D axisxPoint = new Point3D
			{
				X = originPoint.X + tangentVector.DirectionX,
				Y = originPoint.Y + tangentVector.DirectionY,
				Z = originPoint.Z + tangentVector.DirectionZ
			};

			return fullSegment.LocalCoordinateSystem.GetCoordinateSystemMatrix(originPoint, axisxPoint);
		}

		/// <summary>
		/// Calculate local transformation matrix on the polyLine3D at given point
		/// </summary>
		/// <param name="polyLine3D"></param>
		/// <param name="relativePosition"></param>
		/// <returns>Transformation matrix</returns>
		public static IMatrix44 GetTransformation(IPolyLine3D polyLine3D, double relativePosition)
		{
			ISegment3D segment = GetSegmentAtPosition(polyLine3D, relativePosition, out double positionOnSeg, out int inx);
			IMatrix44 lcs = GeomOperation.GetTransformation(segment, positionOnSeg);
			return lcs;
		}

		/// <summary>
		/// Calculate and return the normal Vector of segment
		/// </summary>
		/// <param name="arc">Segment for which normal is calculated</param>
		/// <param name="toleranceLevel">Tolerance Level</param>
		/// <returns>Normal of the segment</returns>
		public static Vector3D GetNormal(IArcSegment3D arc, double toleranceLevel = 1e-6)
		{
			Vector3D vector1 = Subtract(arc.IntermedPoint, arc.StartPoint);
			Vector3D vector2 = Subtract(arc.EndPoint, arc.IntermedPoint);

			if (IsEqual(vector1, vector2, toleranceLevel))
			{
				// TODO : Should be verified
				Vector3D axisZ = new Vector3D(0, 0, 1);
				if (IsEqual(vector2, axisZ))
				{
					vector1 = new Vector3D(1, 0, 0);
				}
				else
				{
					vector1 = axisZ;
				}
			}

			Vector3D normal = vector1 * vector2;
			return normal;
		}

		/// <summary>
		/// Calculate and return the normal Vector of segment
		/// </summary>
		/// <param name="segment">Segment for which normal is calculated</param>
		/// <returns>Normal of the segment</returns>
		public static Vector3D GetNormal(ISegment3D segment)
		{
			if (segment.SegmentType == SegmentType.Line)
			{
				return GetNormal(segment.StartPoint, segment.EndPoint);
			}
			else if ((segment.SegmentType == SegmentType.CircularArc) || (segment.SegmentType == SegmentType.Parabola))
			{
				IArcSegment3D arc = segment as IArcSegment3D;
				if (arc == null)
				{
					throw new InvalidCastException("Wrong segment data");
				}

				return GetNormal(arc, 1e-9);
			}

			throw new NotImplementedException();
		}

		/// <summary>
		/// Calculates the point for a given relative position
		/// </summary>
		/// <param name="segment">Segment on which the operation is done</param>
		/// <param name="relativePosition">relative position for a given unified length</param>
		/// <param name="point">Point for a given input</param>
		/// <returns>True if point exists for a given input</returns>
		public static bool GetPointOnSegment(ISegment3D segment, double relativePosition, ref IPoint3D point)
		{
			if (relativePosition.IsEqual(0))
			{
				point = new Point3D(segment.StartPoint.X, segment.StartPoint.Y, segment.StartPoint.Z);
				return true;
			}

			if (relativePosition.IsEqual(1))
			{
				point = new Point3D(segment.EndPoint.X, segment.EndPoint.Y, segment.EndPoint.Z);
				return true;
			}

			if (segment.SegmentType == SegmentType.Line)
			{
				point = GetLinearPoint(segment.StartPoint, segment.EndPoint, relativePosition);
				return true;
			}
			else if (segment.SegmentType == SegmentType.CircularArc)
			{
				IArcSegment3D arc = segment as IArcSegment3D;
				if (arc == null)
				{
					throw new InvalidCastException("Wrong segment data");
				}

				if (IsCollinear(arc))
				{
					point = Multiply(segment.StartPoint, 1.0 - relativePosition);
					point = Add(point, Multiply(segment.EndPoint, relativePosition));
					return true;
				}
				else
				{
					double arcAngle = relativePosition * GetArcAngle(arc);
					Vector3D vectorX = GetVectorX(segment).Normalize;
					Vector3D vectorY = GetVectorY(segment).Normalize;
					IPoint3D center = new Point3D();
					if (GetCentrePoint(arc, ref center))
					{
						point = Add(center, (vectorX * Math.Cos(arcAngle) + vectorY * Math.Sin(arcAngle)) * GetRadius(arc));
					}
					else
					{
						point = Multiply(segment.StartPoint, 1.0 - relativePosition);
						point = Add(point, Multiply(segment.EndPoint, relativePosition));
					}
				}

				return true;
			}
			else if (segment.SegmentType == SegmentType.Parabola)
			{
				IArcSegment3D segmentParabola = segment as IArcSegment3D;
				if (segmentParabola == null)
				{
					throw new InvalidCastException("Wrong segment data");
				}

				ParabolaSegment3D parabola = segment as ParabolaSegment3D;
				if (parabola == null)
				{
					throw new InvalidCastException("Wrong segment data");
				}

				return parabola.Property.GetPointOnParabola(segmentParabola, relativePosition, ref point);
			}

			throw new NotSupportedException();
		}

		/// <summary>
		/// Calculate the Radius of an arc
		/// </summary>
		/// <param name="arc">Arc for which Radius is calculated</param>
		/// <returns>Radius of the arc</returns>
		public static double GetRadius(IArcSegment3D arc)
		{
			IPoint3D centrePoint = new Point3D();
			if (GetCentrePoint(arc, ref centrePoint))
			{
				return Distance(arc.StartPoint, centrePoint);
			}

			return -1.0;
		}

		/// <summary>
		/// Calculate the Radius of an arc
		/// </summary>
		/// <param name="arc">Arc for which Radius is calculated</param>
		/// <param name="tolerance">tolerance for zero value</param>
		/// <returns>Radius of the arc</returns>
		public static double GetRadius2(IArcSegment3D arc, double tolerance)
		{
			IPoint3D centrePoint = new Point3D();
			if (GetCentrePoint(arc, ref centrePoint, tolerance))
			{
				return Distance(arc.StartPoint, centrePoint);
			}

			return 500.0; //TODO
		}

		/// <summary>
		/// Calculate the Radius of an arc
		/// </summary>
		/// <param name="arc">Arc for which Radius is calculated</param>
		/// <param name="relativePosition">Relative position for parabola</param>
		/// <returns>Radius of the arc</returns>
		public static double GetRadius(IArcSegment3D arc, double relativePosition)
		{
			if (arc is ArcSegment3D)
			{
				return GetRadius(arc);
			}

			if (arc is ParabolaSegment3D)
			{
				double tolerance = MathConstants.ZeroWeak * 2;
				Vector3D vectA = new Vector3D();
				Vector3D vectB = new Vector3D();
				if (relativePosition.IsEqual(0.0))
				{
					relativePosition = tolerance;
				}
				else if (relativePosition.IsEqual(1.0))
				{
					relativePosition -= tolerance;
				}

				GetTangentOnSegment(arc, relativePosition - tolerance, ref vectA);
				GetTangentOnSegment(arc, relativePosition + tolerance, ref vectB);
				double angle = GetAngle(vectA, vectB);
				double len = GetLength(arc);
				len = len * tolerance * 2;
				double rad = len / angle;

				//IPoint3D pointA = new Point3D();
				//IPoint3D pointB = new Point3D();
				//IPoint3D pointC = new Point3D();
				//tolerance = 1e-4;
				//if (relativePosition < tolerance)
				//{
				//	relativePosition += tolerance;
				//}

				////GetPointOnParamS(sign, relativePosition - dt, max_t, &P1);
				//GetPointOnSegment(arc, relativePosition - tolerance, ref pointA);
				////GetPointOnParamS(sign, relativePosition, max_t, &P2);
				//GetPointOnSegment(arc, relativePosition, ref pointB);
				////GetPointOnParamS(sign, relativePosition + dt, max_t, &P3);
				//GetPointOnSegment(arc, relativePosition + tolerance, ref pointC);

				////*pVal = 1e8 * (P3 + P1 - 2 * P2);
				//Vector3D vect = GeomOperation.Multiply(GeomOperation.Subtract(GeomOperation.Add(pointC, pointA), GeomOperation.Multiply(pointB, 2.0)), 1e8);
				//double k = vect.Magnitude;
				return rad;
			}

			return -1.0;
		}

		/// <summary>
		/// Calculate the Radius of an arc
		/// </summary>
		/// <param name="arc">Arc for which Radius is calculated</param>
		/// <param name="relativePosition">Relative position for parabola</param>
		/// <param name="tolerance">tolerance for zero value</param>
		/// <returns>Radius of the arc</returns>
		public static double GetRadius2(IArcSegment3D arc, double relativePosition, double tolerance)
		{
			if (arc is ArcSegment3D)
			{
				return GetRadius(arc, tolerance);
			}

			if (arc is ParabolaSegment3D)
			{
				//double tolerance = MathConstants.ZeroWeak * 2;
				Vector3D vectA = new Vector3D();
				Vector3D vectB = new Vector3D();
				if (relativePosition.IsEqual(0.0))
				{
					relativePosition = tolerance;
				}
				else if (relativePosition.IsEqual(1.0))
				{
					relativePosition -= tolerance;
				}

				GetTangentOnSegment(arc, relativePosition - tolerance, ref vectA);
				GetTangentOnSegment(arc, relativePosition + tolerance, ref vectB);
				double angle = GetAngle(vectA, vectB);
				double len = GetLength(arc);
				len = len * tolerance * 2;
				double rad = len / angle;

				//IPoint3D pointA = new Point3D();
				//IPoint3D pointB = new Point3D();
				//IPoint3D pointC = new Point3D();
				//tolerance = 1e-4;
				//if (relativePosition < tolerance)
				//{
				//	relativePosition += tolerance;
				//}

				////GetPointOnParamS(sign, relativePosition - dt, max_t, &P1);
				//GetPointOnSegment(arc, relativePosition - tolerance, ref pointA);
				////GetPointOnParamS(sign, relativePosition, max_t, &P2);
				//GetPointOnSegment(arc, relativePosition, ref pointB);
				////GetPointOnParamS(sign, relativePosition + dt, max_t, &P3);
				//GetPointOnSegment(arc, relativePosition + tolerance, ref pointC);

				////*pVal = 1e8 * (P3 + P1 - 2 * P2);
				//Vector3D vect = GeomOperation.Multiply(GeomOperation.Subtract(GeomOperation.Add(pointC, pointA), GeomOperation.Multiply(pointB, 2.0)), 1e8);
				//double k = vect.Magnitude;
				return rad;
			}

			return 500.0; //TODO
		}

		/// <summary>
		/// Calculate and return the local X Axis of the segment
		/// </summary>
		/// <param name="segment">Segment for which X axis is calculated</param>
		/// <returns>Local X axis of the segment</returns>
		public static Vector3D GetVectorX(ISegment3D segment)
		{
			if (segment == null || !IsValid(segment))
			{
				return new Vector3D();
			}

			if (segment.SegmentType == SegmentType.Line)
			{
				Vector3D vectorX = new Vector3D();
				vectorX = Subtract(segment.EndPoint, segment.StartPoint).Normalize;
				return vectorX;
			}
			else if (segment.SegmentType == SegmentType.CircularArc)
			{
				IArcSegment3D arc = segment as IArcSegment3D;
				if (arc == null)
				{
					throw new InvalidCastException("Wrong segment data");
				}

				IPoint3D centre = new Point3D();
				GetCentrePoint(arc, ref centre);
				return Subtract(arc.StartPoint, centre);
			}

			throw new NotImplementedException();
		}

		/// <summary>
		/// Calculate and return the local Y Axis of the segment
		/// </summary>
		/// <param name="segment">Segment for which Y axis is calculated</param>
		/// <returns>Local Y axis of the segment</returns>
		public static Vector3D GetVectorY(ISegment3D segment)
		{
			if (segment == null || !IsValid(segment))
			{
				return new Vector3D();
			}

			return GetNormal(segment) * GetVectorX(segment);
		}

		/// <summary>
		/// Check whether a Circular Arc and Line intersects and returns a intersection point.
		/// </summary>
		/// <param name="arc">Circular Arc to be compared for intersection</param>
		/// <param name="line">Line to be compared for intersection</param>
		/// <param name="intersectionPoints">Intersection Points of a Curve</param>
		/// <param name="relativePosition0">Relative Position for this Circular Arc</param>
		/// <param name="relativePosition1">Relative Position for Line</param>
		/// <returns>return true if Circular Arc and Line intersects</returns>
		public static bool Intersect(IArcSegment3D arc, ILineSegment3D line, ref IList<IPoint3D> intersectionPoints, ref IList<double> relativePosition0, ref IList<double> relativePosition1)
		{
			IPoint3D a = line.StartPoint;
			IPoint3D b = line.EndPoint;

			Vector3D x, y, z;
			IPoint3D c = new Point3D();
			GetCentrePoint(arc, ref c);
			x = GetVectorX(arc);
			y = GetVectorY(arc);
			z = GetNormal(arc, 1e-9);
			double radius = GetRadius(arc);
			if (radius < 0)
			{
				return false;
			}

			double val = z | Subtract(a, c);
			if (!val.IsZero(MathConstants.ZeroMaximum))
			{
				return false;
			}

			val = z | Subtract(b, c);
			if (!val.IsZero(MathConstants.ZeroMaximum))
			{
				return false;
			}

			Vector3D vector1 = Subtract(b, a);
			Vector3D vectorAC = Subtract(c, a);

			double magnitudeOfLine = vector1.Magnitude;

			if (magnitudeOfLine.Equals(0.0))
			{
				return true;
			}

			double vAC = vectorAC.Magnitude; // inflatePoint - Centre
			double vAX = (vectorAC | vector1) / magnitudeOfLine;
			double vCX = (vAC * vAC) - (vAX * vAX);

			double rr = radius * radius;
			double d = rr - vCX;
			if (d.IsLesser(0.0, MathConstants.ZeroMaximum))
			{
				return true;
			}

			double angle = GetArcAngle(arc);

			if (d.IsZero(MathConstants.ZeroMaximum))
			{
				vAX /= magnitudeOfLine;
				IPoint3D rx = Add(a, vector1 * vAX);
				Vector3D crx = Subtract(rx, c);
				double t = NormalizeCircleParameter(angle, crx, x, y);
				if (relativePosition0 != null)
				{
					relativePosition0.Add(vAX);
				}

				if (relativePosition1 != null)
				{
					relativePosition1.Add(t);
				}

				if (intersectionPoints != null)
				{
					intersectionPoints.Add(rx);
				}

				return true;
			}

			d = Math.Sqrt(d);
			double t00 = (vAX + d) / magnitudeOfLine;
			double t01 = (vAX - d) / magnitudeOfLine;

			IPoint3D point0 = Add(a, vector1 * t00);
			IPoint3D point1 = Add(a, vector1 * t01);

			if (intersectionPoints != null)
			{
				intersectionPoints.Add(point0);
				intersectionPoints.Add(point1);
			}

			double t10 = NormalizeCircleParameter(angle, Subtract(point0, c), x, y);
			double t11 = NormalizeCircleParameter(angle, Subtract(point1, c), x, y);

			if (relativePosition0 != null)
			{
				relativePosition0.Add(t00);
				relativePosition0.Add(t01);
			}

			if (relativePosition1 != null)
			{
				relativePosition1.Add(t10);
				relativePosition1.Add(t11);
			}

			return true;
		}

		/// <summary>
		/// Calculate Intersection Point for two Segements
		/// </summary>
		/// <param name="segment1">Segment1</param>
		/// <param name="segment2">Segment2</param>
		/// <returns>List of intersection</returns>
		public static List<IPoint3D> IntersectSegment(ISegment3D segment1, ISegment3D segment2)
		{
			List<IPoint3D> listOfIntersectionPoint = new List<IPoint3D>();
			if ((segment1.SegmentType == SegmentType.Line) && (segment2.SegmentType == SegmentType.Line))
			{
				IPoint3D intersectionPoint = new Point3D();
				if (GeomOperation.IntersectSegment(segment1 as LineSegment3D, segment2 as LineSegment3D, ref intersectionPoint))
				{
					listOfIntersectionPoint.Add(intersectionPoint);
				}
			}
			else if ((segment1.SegmentType == SegmentType.Line) && (segment2.SegmentType == SegmentType.CircularArc))
			{
				IList<IPoint3D> intersectionPoint = new List<IPoint3D>();
				IList<double> relativePosition1 = new List<double>();
				IList<double> relativePosition2 = new List<double>();
				if (GeomOperation.Intersect(segment2 as IArcSegment3D, segment1 as ILineSegment3D, ref intersectionPoint, ref relativePosition1, ref relativePosition2))
				{
					foreach (IPoint3D intersectPoint in intersectionPoint)
					{
						double relPosition1 = 0;
						double relPosition2 = 0;
						if (IsPointOnSegment(segment1, intersectPoint, ref relPosition1) && IsPointOnSegment(segment2, intersectPoint, ref relPosition2))
						{
							listOfIntersectionPoint.Add(intersectPoint);
						}
					}
				}
			}
			else if ((segment1.SegmentType == SegmentType.CircularArc) && (segment2.SegmentType == SegmentType.Line))
			{
				IList<IPoint3D> intersectionPoint = new List<IPoint3D>();
				IList<double> relativePosition1 = new List<double>();
				IList<double> relativePosition2 = new List<double>();
				if (GeomOperation.Intersect(segment1 as IArcSegment3D, segment2 as ILineSegment3D, ref intersectionPoint, ref relativePosition1, ref relativePosition2))
				{
					foreach (IPoint3D intersectPoint in intersectionPoint)
					{
						double relPosition1 = 0;
						double relPosition2 = 0;
						if (IsPointOnSegment(segment1, intersectPoint, ref relPosition1) && IsPointOnSegment(segment2, intersectPoint, ref relPosition2))
						{
							listOfIntersectionPoint.Add(intersectPoint);
						}
					}
				}
			}
			else if ((segment1.SegmentType == SegmentType.CircularArc) && (segment2.SegmentType == SegmentType.CircularArc))
			{
				IList<IPoint3D> intersectionPoint = new List<IPoint3D>();
				IList<double> relativePosition1 = new List<double>();
				IList<double> relativePosition2 = new List<double>();
				if (GeomOperation.IntersectSegment(segment1 as IArcSegment3D, segment2 as IArcSegment3D, ref intersectionPoint, ref relativePosition1, ref relativePosition2))
				{
					foreach (IPoint3D intersectPoint in intersectionPoint)
					{
						double relPosition1 = 0;
						double relPosition2 = 0;
						if (IsPointOnSegment(segment1, intersectPoint, ref relPosition1) && IsPointOnSegment(segment2, intersectPoint, ref relPosition2))
						{
							listOfIntersectionPoint.Add(intersectPoint);
						}
					}
				}
			}

			return listOfIntersectionPoint;
		}

		/// <summary>
		/// Check whether two segments extend intersects and calculate their relative position on segment
		/// </summary>
		/// <param name="segment1">Segment1</param>
		/// <param name="segment2">Segment2</param>
		/// <param name="relativePositions1">Relative Positions for Segment1</param>
		/// <param name="relativePositions2">Relative Positions for Segment2</param>
		/// <returns>True if two segments extend intersects</returns>
		public static bool IntersectSegment(ISegment3D segment1, ISegment3D segment2, List<double> relativePositions1, List<double> relativePositions2)
		{
			// TODO: Use ICollection instead of IList
			// TODO: for relativePositions1 and relativePositions2 : Check for null just beofre using. If user want only one list, there is no need to prepare both
			if (segment1 == null || segment2 == null || relativePositions1 == null || relativePositions2 == null)
			{
				return false;
			}

			if ((segment1.SegmentType == SegmentType.Line) && (segment2.SegmentType == SegmentType.Line))
			{
				IPoint3D intersectionPoint = new Point3D();
				double relativePosition0 = 0, relativePosition1 = 0;
				bool lineParallel = false;
				if (!GeomOperation.IntersectLine(segment1 as LineSegment3D, segment2 as LineSegment3D, ref intersectionPoint, ref relativePosition0, ref relativePosition1, ref lineParallel))
				{
					return false;
				}

				relativePositions1.Add(relativePosition0);
				relativePositions2.Add(relativePosition1);
			}
			else if ((segment1.SegmentType == SegmentType.Line) && (segment2.SegmentType == SegmentType.CircularArc))
			{
				IList<IPoint3D> intersectionPoint = new List<IPoint3D>();
				IList<double> relativePosition1 = new List<double>(); // TODO: Why Seperate list is created. Cann't pass the original list?
				IList<double> relativePosition2 = new List<double>();
				GeomOperation.Intersect(segment2 as IArcSegment3D, segment1 as ILineSegment3D, ref intersectionPoint, ref relativePosition1, ref relativePosition2);
				relativePositions1.AddRange(relativePosition2);
				relativePositions2.AddRange(relativePosition1);
			}
			else if ((segment1.SegmentType == SegmentType.CircularArc) && (segment2.SegmentType == SegmentType.Line))
			{
				IList<IPoint3D> intersectionPoint = new List<IPoint3D>();
				IList<double> relativePosition1 = new List<double>();
				IList<double> relativePosition2 = new List<double>();
				GeomOperation.Intersect(segment1 as IArcSegment3D, segment2 as ILineSegment3D, ref intersectionPoint, ref relativePosition1, ref relativePosition2);
				relativePositions1.AddRange(relativePosition1);
				relativePositions2.AddRange(relativePosition2);
			}
			else if ((segment1.SegmentType == SegmentType.CircularArc) && (segment2.SegmentType == SegmentType.CircularArc))
			{
				IList<IPoint3D> intersectionPoint = new List<IPoint3D>();
				IList<double> relativePosition1 = new List<double>();
				IList<double> relativePosition2 = new List<double>();
				GeomOperation.IntersectSegment(segment1 as IArcSegment3D, segment2 as IArcSegment3D, ref intersectionPoint, ref relativePosition1, ref relativePosition2);
				relativePositions1.AddRange(relativePosition1);
				relativePositions2.AddRange(relativePosition2);
			}

			if (relativePositions1.Count > 0 || relativePositions2.Count > 0)
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Check the given vector is upwards
		/// </summary>
		/// <param name="vector">Input Vector3D</param>
		/// <returns>True if the vector is upwards</returns>
		public static bool IsUpwards(Vector3D vector)
		{
			double value = vector | (new Vector3D(0, 0, 1));
			if (value.IsZero())
			{
				value = vector | (new Vector3D(1, 0, 0));
				if (value.IsZero())
				{
					value = vector | (new Vector3D(0, 1, 0));
				}
			}

			if (value.IsLesser(0))
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Check the given arc is valid
		/// </summary>
		/// <param name="arc">Input arc</param>
		/// <returns>True if the arc is valid</returns>
		public static bool IsValid(IArcSegment3D arc)
		{
			if (IsEqual(arc.StartPoint, arc.IntermedPoint))
			{
				return false;
			}
			else if (IsEqual(arc.IntermedPoint, arc.EndPoint))
			{
				return false;
			}
			else if (IsEqual(arc.StartPoint, arc.EndPoint))
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Check the given line is valid
		/// </summary>
		/// <param name="line">Input line</param>
		/// <returns>True if the line is valid</returns>
		public static bool IsValid(ILineSegment3D line)
		{
			return GetLength(line).IsGreater(0);
		}

		/// <summary>
		/// Check whether segment is valid.
		/// </summary>
		/// <param name="segment">Segment</param>
		/// <returns>True if segment is valid</returns>
		public static bool IsValid(ISegment3D segment)
		{
			if (segment.SegmentType == SegmentType.Line)
			{
				ILineSegment3D line = segment as ILineSegment3D;
				if (line == null)
				{
					throw new InvalidCastException("Wrong segment data");
				}

				return IsValid(line);
			}
			else if ((segment.SegmentType == SegmentType.CircularArc) || (segment.SegmentType == SegmentType.Parabola))
			{
				IArcSegment3D arc = segment as IArcSegment3D;
				if (arc == null)
				{
					throw new InvalidCastException("Wrong segment data");
				}

				return IsValid(arc);
			}

			throw new NotSupportedException();
		}

		/// <summary>
		/// Checks equality of given two segments
		/// </summary>
		/// <param name="segment1">First segment</param>
		/// <param name="segment2">Second segment</param>
		/// <returns>True if both segments are equal</returns>
		public static bool IsEqual(ISegment3D segment1, ISegment3D segment2)
		{
			if (segment1.GetType() == segment2.GetType())
			{
				if (segment1.GetType().Equals(typeof(ArcSegment3D)))
				{
					ArcSegment3D arc1 = segment1 as ArcSegment3D;
					ArcSegment3D arc2 = segment2 as ArcSegment3D;
					if (!IsEqual(arc1.IntermedPoint, arc2.IntermedPoint))
					{
						return false;
					}
				}

				if (segment1.GetType().Equals(typeof(ParabolaSegment3D)))
				{
					ParabolaSegment3D arc1 = segment1 as ParabolaSegment3D;
					ParabolaSegment3D arc2 = segment2 as ParabolaSegment3D;
					if (!IsEqual(arc1.IntermedPoint, arc2.IntermedPoint))
					{
						return false;
					}
				}

				if (IsEqual(segment1.StartPoint, segment2.StartPoint))
				{
					if (IsEqual(segment1.EndPoint, segment2.EndPoint))
					{
						return true;
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Checks equality of given two segments with given tolerance
		/// </summary>
		/// <param name="segment1">First segment</param>
		/// <param name="segment2">Second segment</param>
		/// <param name="precision">Tolerance = Precision set</param>
		/// <returns>True if both segments are equal</returns>
		public static bool IsEqualWithTolerance(ISegment3D segment1, ISegment3D segment2, double precision = MathConstants.ZeroGeneral)
		{
			if (segment1.GetType() == segment2.GetType())
			{
				if (segment1.GetType().Equals(typeof(ArcSegment3D)))
				{
					ArcSegment3D arc1 = segment1 as ArcSegment3D;
					ArcSegment3D arc2 = segment2 as ArcSegment3D;
					if (!IsEqual(arc1.IntermedPoint, arc2.IntermedPoint, precision))
					{
						return false;
					}
				}

				if (segment1.GetType().Equals(typeof(ParabolaSegment3D)))
				{
					ParabolaSegment3D arc1 = segment1 as ParabolaSegment3D;
					ParabolaSegment3D arc2 = segment2 as ParabolaSegment3D;
					if (!IsEqual(arc1.IntermedPoint, arc2.IntermedPoint, precision))
					{
						return false;
					}
				}

				if (IsEqual(segment1.StartPoint, segment2.StartPoint, precision))
				{
					if (IsEqual(segment1.EndPoint, segment2.EndPoint, precision))
					{
						return true;
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Commpares coordinates of the begin and end points. (line AB == line BA)
		/// </summary>
		/// <param name="segment1">Segment 1</param>
		/// <param name="segment2">Segment 2</param>
		/// <param name="tolerance"></param>
		/// <returns>It returns true if line segments are at the same position</returns>
		public static bool AreAtSamePosition(ILineSegment3D segment1, ILineSegment3D segment2, double tolerance = MathConstants.ZeroGeneral)
		{
			if (IsEqual(segment1.EndPoint, segment2.StartPoint, tolerance))
			{
				if (IsEqual(segment1.StartPoint, segment2.EndPoint, tolerance))
				{
					return true;
				}
			}

			if (IsEqual(segment1.StartPoint, segment2.StartPoint, tolerance))
			{
				if (IsEqual(segment1.EndPoint, segment2.EndPoint, tolerance))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Check whether two lines intersect and returns an intersection point.
		/// </summary>
		/// <param name="line0">Line0 to be compared for intersection</param>
		/// <param name="line1">Line1 to be compared for intersection</param>
		/// <param name="intersectionPoint">Intersection Point of two Lines</param>
		/// <param name="relativePosition0">Relative Position for Line1</param>
		/// <param name="relativePosition1">Relative Position for Line2 </param>
		/// <param name="lineParallel">check Lines are parallel</param>
		/// <param name="precision">precision</param>
		/// <returns>return true if two Lines intersects</returns>
		public static bool IntersectLine(ILineSegment3D line0, ILineSegment3D line1, ref IPoint3D intersectionPoint, ref double relativePosition0, ref double relativePosition1, ref bool lineParallel, double precision = MathConstants.ZeroMaximum)
		{
			if (GetIntersectionOfLines(line0, line1, ref relativePosition0, ref relativePosition1, ref lineParallel, precision) > 0)
			{
				GetPointOnSegment(line0, relativePosition0, ref intersectionPoint);
				return true;
			}

			return false;
		}

		/// <summary>
		/// Check whether two segments intersect and returns an intersection point.
		/// </summary>
		/// <param name="line0">Line0 to be compared for intersection</param>
		/// <param name="line1">Line1 to be compared for intersection</param>
		/// <param name="intersectionPoint">Intersection Point of two Lines</param>
		/// <param name="precision">precision</param>
		/// <returns>return true if two lines intersect</returns>
		public static bool IntersectSegment(ILineSegment3D line0, ILineSegment3D line1, ref IPoint3D intersectionPoint, double precision = MathConstants.ZeroMaximum)
		{
			double relativePosition0 = 0.0, relativePosition1 = 0.0;
			bool lineParallel = false;
			if (GetIntersectionOfLines(line0, line1, ref relativePosition0, ref relativePosition1, ref lineParallel, precision) > 0)
			{
				if (IsValidRelativePosition(relativePosition0) && IsValidRelativePosition(relativePosition1))
				{
					GetPointOnSegment(line0, relativePosition0, ref intersectionPoint);
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Check whether line and segment intersect and returns an intersection point.
		/// </summary>
		/// <param name="line">Line to be compared for intersection</param>
		/// <param name="seg">Segment to be compared for intersection</param>
		/// <param name="intersectionPoint">Intersection Point of two Lines</param>
		/// <param name="precision">precision</param>
		/// <returns>return true if line and segment intersect</returns>
		public static bool IntersectLineAndSegment(ILineSegment3D line, ILineSegment3D seg, ref IPoint3D intersectionPoint, double precision = MathConstants.ZeroMaximum)
		{
			double relativePositionLine = 0.0, relativePositionSeg = 0.0;
			bool lineParallel = false;
			if (GetIntersectionOfLines(line, seg, ref relativePositionLine, ref relativePositionSeg, ref lineParallel, precision) > 0)
			{
				if (IsValidRelativePosition(relativePositionSeg))
				{
					GetPointOnSegment(seg, relativePositionSeg, ref intersectionPoint);
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Calculates the tangent for segment for a given offset.
		/// </summary>
		/// <param name="segment">Segment</param>
		/// <param name="relativePosition">relative position of segment</param>
		/// <param name="tangentVector">tangent vector segment</param>
		/// <returns>true if tangent Vector exists for a given segment</returns>
		public static bool GetTangentOnSegment(ISegment3D segment, double relativePosition, ref Vector3D tangentVector)
		{
			if (segment.SegmentType == SegmentType.Line)
			{
				tangentVector = Subtract(segment.EndPoint, segment.StartPoint).Normalize;
			}
			else if (segment.SegmentType == SegmentType.CircularArc)
			{
				IPoint3D point = null;
				GetPointOnSegment(segment, relativePosition, ref point);
				IPoint3D centrePoint = null;
				GetCentrePoint(segment as IArcSegment3D, ref centrePoint);
				Vector3D c = Subtract(centrePoint, point);//.Normalize;

				Vector3D n = GetNormal(segment as IArcSegment3D, 1e-9);
				tangentVector = (c * n).Normalize;

				//Vector3D a = Subtract(centrePoint, point);
				//if (GeomOperation.IsEqual(segment.StartPoint, point))
				//{
				//  a = Subtract(centrePoint, point);//.Normalize;
				//}
				//else
				//{
				//  a = Subtract(segment.StartPoint, point);//.Normalize;
				//}

				//Vector3D b = Subtract(centrePoint, point);
				//if (GeomOperation.IsEqual(segment.EndPoint, point))
				//{
				//  b = Subtract(centrePoint, point);//.Normalize;
				//}
				//else
				//{
				//  b = Subtract(segment.EndPoint, point);//.Normalize;
				//}

				//if (IsCollinear(a, b))
				//{
				//  tangentVector = b;
				//}
				//else
				//{
				//  Vector3D n = a * b;
				//  //IPoint3D centrePoint = null;
				//  //GetCentrePoint(segment as IArcSegment3D, ref centrePoint);
				//  //Vector3D c = Subtract(centrePoint, point);//.Normalize;
				//  tangentVector = (n * c).Normalize;
				//}
			}
			else
			{
				IPoint3D tangentStartPoint = new Point3D();
				IPoint3D tangentEndPoint = new Point3D();

				double tolerance = MathConstants.ZeroWeak;
				if ((relativePosition + tolerance).IsGreater(1))
				{
					tolerance = -MathConstants.ZeroWeak;
				}

				GetPointOnSegment(segment, relativePosition - tolerance, ref tangentStartPoint);
				GetPointOnSegment(segment, relativePosition + tolerance, ref tangentEndPoint);

				tangentVector = (0.5 / tolerance) * Subtract(tangentEndPoint, tangentStartPoint);
				tangentVector = tangentVector.Normalize;
			}

			return true;
		}

		/// <summary>
		/// Verify whether point lies on segment.
		/// </summary>
		/// <param name="segment">Segment</param>
		/// <param name="pointOnSegment">Point On segment to be verified</param>
		/// <param name="relativePosition">Relative Position of segment</param>
		/// <param name="toleranceLevel">Tolerance pevel (absolute position check)</param>
		/// <param name="relativeValTolerance">Tolerance level (relative position check)</param>
		/// <param name="takeendtolerance">Take tolerance also for end points</param>
		/// <returns>True if point is on segment</returns>
		public static bool IsPointOnSegment(ISegment3D segment, IPoint3D pointOnSegment, ref double relativePosition, double toleranceLevel = MathConstants.ZeroWeak, double relativeValTolerance = MathConstants.ZeroWeak, bool takeendtolerance = false)
		{
			double endTolerance = MathConstants.ZeroWeak;
			if (takeendtolerance)
			{
				endTolerance = toleranceLevel;
			}

			if (IsEqual(pointOnSegment, segment.StartPoint, endTolerance))
			{
				relativePosition = 0.0;
				return true;
			}

			if (IsEqual(pointOnSegment, segment.EndPoint, endTolerance))
			{
				relativePosition = 1.0;
				return true;
			}

			if (segment.SegmentType == SegmentType.Line)
			{
				Vector3D vs = GeomOperation.Subtract(segment.EndPoint, segment.StartPoint);
				Vector3D vp = GeomOperation.Subtract(pointOnSegment, segment.StartPoint);
				if (GeomOperation.IsCollinear(vs, vp, relativeValTolerance))
				{
					double len = GeomOperation.GetLength(segment);
					double lb = GeomOperation.Distance(segment.StartPoint, pointOnSegment);
					double le = GeomOperation.Distance(segment.EndPoint, pointOnSegment);
					double ll = lb + le;
					relativePosition = lb / len;
					return len.IsEqual(ll, toleranceLevel);
				}
				return false;
			}
			else
			{
				if (segment.SegmentType == SegmentType.CircularArc)
				{
					IPoint3D centrePoint = null;
					if (GetCentrePoint(segment as IArcSegment3D, ref centrePoint))
					{
						double lb = GeomOperation.Distance(segment.StartPoint, centrePoint);
						double lp = GeomOperation.Distance(pointOnSegment, centrePoint);
						if (!lb.IsEqual(lp, toleranceLevel))
						{
							return false;
						}
					}
				}

				IList<double> resultCollection = FindSolution(segment, pointOnSegment, false);
				if (resultCollection.Count > 0)
				{
					IPoint3D outPt = new Point3D();
					IPoint3D paramPt = new Point3D();
					double relativeOffset = GetNearestPoint(pointOnSegment, segment, resultCollection, ref outPt);
					GetPointOnSegment(segment, relativeOffset, ref paramPt);
					double arcLength = GetLength(segment);
					bool isRelativePositionOnCurve = IsRelativePositionOnSegment(relativeOffset, arcLength, relativeValTolerance);
					bool isEqual = IsEqual(pointOnSegment, paramPt, toleranceLevel);
					if (isRelativePositionOnCurve && isEqual)
					{
						relativePosition = relativeOffset;
						return true;
					}
				}

				return false;
			}
		}

		/// <summary>
		/// Check whether two segments intersects.
		/// </summary>
		/// <param name="segment1">Segment1</param>
		/// <param name="segment2">Segment2</param>
		/// <returns>True if segment intersects</returns>
		public static bool IsSegmentIntersects(ISegment3D segment1, ISegment3D segment2)
		{
			if ((segment1.SegmentType == SegmentType.Line) && (segment2.SegmentType == SegmentType.Line))
			{
				return IsSegmentIntersects(segment1 as ILineSegment3D, segment2 as ILineSegment3D);
			}
			else if ((segment1.SegmentType == SegmentType.CircularArc) && (segment2.SegmentType == SegmentType.CircularArc))
			{
				return IsSegmentIntersects(segment1 as IArcSegment3D, segment2 as IArcSegment3D);
			}
			else if ((segment1.SegmentType == SegmentType.Line) && (segment2.SegmentType == SegmentType.CircularArc))
			{
				return IsSegmentIntersects(segment1 as ILineSegment3D, segment2 as IArcSegment3D);
			}
			else if ((segment1.SegmentType == SegmentType.CircularArc) && (segment2.SegmentType == SegmentType.Line))
			{
				return IsSegmentIntersects(segment2 as ILineSegment3D, segment1 as IArcSegment3D);
			}

			throw new NotImplementedException(segment1.SegmentType.ToString() + "," + segment2.SegmentType.ToString());
		}

		/// <summary>
		/// Check whether two Line segments intersects.
		/// </summary>
		/// <param name="lineSegment1">Line Segment1</param>
		/// <param name="lineSegment2">Line Segment2</param>
		/// <returns>True if segment intersects</returns>
		public static bool IsSegmentIntersects(ILineSegment3D lineSegment1, ILineSegment3D lineSegment2)
		{
			IPoint3D intersectionPoints = new Point3D();
			double relativePosition1 = 0, relativePosition2 = 0;
			bool isParallel = false;
			if (IntersectLine(lineSegment1 as ILineSegment3D, lineSegment2 as ILineSegment3D, ref intersectionPoints, ref relativePosition1, ref relativePosition2, ref isParallel))
			{
				if (relativePosition1.IsGreater(0) && relativePosition1.IsLesser(1))
				{
					if (relativePosition2.IsGreater(0) && relativePosition2.IsLesser(1))
					{
						return true;
					}
				}

				return false;
			}

			return false;
		}

		/// <summary>
		/// Check whether two arc segment intersects.
		/// </summary>
		/// <param name="arcSegment1">Arc Segment1</param>
		/// <param name="arcSegment2">Arc Segment2</param>
		/// <returns>True if two arc segment intersects</returns>
		public static bool IsSegmentIntersects(IArcSegment3D arcSegment1, IArcSegment3D arcSegment2)
		{
			IList<IPoint3D> intersectionPoints = new List<IPoint3D>();
			IList<double> relativePosition1 = new List<double>();
			IList<double> relativePosition2 = new List<double>();
			if (IntersectSegment(arcSegment1 as IArcSegment3D, arcSegment2 as IArcSegment3D, ref intersectionPoints, ref relativePosition1, ref relativePosition2))
			{
				for (int i = 0; i < relativePosition1.Count; i++)
				{
					if (IsPositionOnSegment(relativePosition1[i], relativePosition2[i]))
					{
						return true;
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Check whether Linesegment and Arcsegment intersects.
		/// </summary>
		/// <param name="lineSegment1">Line Segment</param>
		/// <param name="arcSegment2">Arc Segment</param>
		/// <returns>True if Arc and Line segment intersects</returns>
		public static bool IsSegmentIntersects(ILineSegment3D lineSegment1, IArcSegment3D arcSegment2)
		{
			IList<IPoint3D> intersectionPoints = new List<IPoint3D>();
			IList<double> relativePosition1 = new List<double>();
			IList<double> relativePosition2 = new List<double>();
			if (Intersect(arcSegment2 as IArcSegment3D, lineSegment1 as ILineSegment3D, ref intersectionPoints, ref relativePosition1, ref relativePosition2))
			{
				for (int i = 0; i < relativePosition1.Count; i++)
				{
					if (IsPositionOnSegment(relativePosition1[i], relativePosition2[i]))
					{
						return true;
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Check whether two Circular Arc intersects and calculate its intersection points.
		/// </summary>
		/// <param name="circularArc0">Circular Arc0 to be compared for intersection</param>
		/// <param name="circularArc1">Circular Arc1 to be compared for intersection</param>
		/// <param name="intersectionPoint">Intersection Points of a two Arc</param>
		/// <param name="relativePosition0">Relative Position for circularArc0</param>
		/// <param name="relativePosition1">Relative Position for a given circularArc1</param>
		/// <returns>return true if two Circular Arc intersects</returns>
		public static bool IntersectSegment(IArcSegment3D circularArc0, IArcSegment3D circularArc1, ref IList<IPoint3D> intersectionPoint, ref IList<double> relativePosition0, ref IList<double> relativePosition1)
		{
			// TODO: Use ICollection instead of IList
			// TODO: Remove ref before list. We are not creating any list. We are just adding elements only
			double radius0 = GetRadius(circularArc0);
			IPoint3D c0 = null;
			GetCentrePoint(circularArc0, ref c0);
			Vector3D x0 = GetVectorX(circularArc0);
			Vector3D y0 = GetVectorY(circularArc0);
			Vector3D z0 = GetNormal(circularArc0, 1e-9);

			double radius1 = GetRadius(circularArc1);
			IPoint3D c1 = null;
			GetCentrePoint(circularArc1, ref c1);
			Vector3D x1 = GetVectorX(circularArc1);
			Vector3D y1 = GetVectorY(circularArc1);
			Vector3D z1 = GetNormal(circularArc1, 1e-9);

			if (radius0.IsLesser(0) || radius1.IsLesser(0))
			{
				return false;
			}

			Vector3D cc = Subtract(c1, c0);

			if (!(z0 * z1).Magnitude.IsZero())
			{
				return false;
			}

			if (!(cc | z0).IsZero())
			{
				return false;
			}

			double d = ~cc;

			if (d.IsZero(MathConstants.ZeroWeak))
			{
				if (!(radius0 - radius1).IsZero())
				{
					return true;
				}

				bool bIs;
				double param;
				IPoint3D point = null;

				param = 0.0;
				GetPointOnSegment(circularArc0, 0, ref point);
				bIs = IsPointOnSegment(circularArc1, point, ref param);
				if (bIs || param != 0.0)
				{
					relativePosition0.Add(0.0);
					relativePosition1.Add(param);
					intersectionPoint.Add(point);
				}

				param = 0.0;
				GetPointOnSegment(circularArc0, 1, ref point);
				bIs = IsPointOnSegment(circularArc1, point, ref param);
				if (bIs || (!param.IsEqual(0)))
				{
					relativePosition0.Add(1.0);
					relativePosition1.Add(param);
					intersectionPoint.Add(point);
				}

				param = 0.0;
				GetPointOnSegment(circularArc1, 0, ref point);
				bIs = IsPointOnSegment(circularArc0, point, ref param);
				if (bIs || param != 0.0)
				{
					relativePosition0.Add(param);
					relativePosition1.Add(0.0);
					intersectionPoint.Add(point);
				}

				param = 0.0;
				GetPointOnSegment(circularArc1, 1, ref point);
				bIs = IsPointOnSegment(circularArc0, point, ref param);
				if (bIs || param != 0.0)
				{
					relativePosition0.Add(param);
					relativePosition1.Add(1.0);
					intersectionPoint.Add(point);
				}

				return true;
			}

			radius0 *= radius0;
			radius1 *= radius1;

			double d0 = (((radius0 - radius1) / d) + d) / 2.0;
			double d1 = -(((radius1 - radius0) / d) + d) / 2.0;

			double xx = radius0 - (d0 * d0);
			if (xx.IsZero())
			{
				Vector3D n = (z0 * cc).Normalize;
				cc = cc.Normalize;
				Vector3D v0 = d0 * cc;
				Vector3D v1 = d1 * cc;

				double angle0 = GetArcAngle(circularArc0);
				double angle1 = GetArcAngle(circularArc1);

				double t0 = NormalizeCircleParameter(angle0, v0, x0, y0);
				double t1 = NormalizeCircleParameter(angle1, v1, x1, y1);

				intersectionPoint.Add(Add(c0, v0));
				relativePosition0.Add(t0);
				relativePosition1.Add(t1);
				return true;
			}
			else if (xx.IsLesser(0))
			{
				return true;
			}

			double x = Math.Sqrt(xx);

			Vector3D norm = (z0 * cc).Normalize;
			cc = cc.Normalize;
			Vector3D v00 = (d0 * cc) + (x * norm);
			Vector3D v01 = (d0 * cc) - (x * norm);

			Vector3D v10 = (d1 * cc) + (x * norm);
			Vector3D v11 = (d1 * cc) - (x * norm);

			double arcAngle0 = GetArcAngle(circularArc0);
			double arcAngle1 = GetArcAngle(circularArc1);

			double t00 = NormalizeCircleParameter(arcAngle0, v00, x0, y0);
			double t01 = NormalizeCircleParameter(arcAngle0, v01, x0, y0);

			double t10 = NormalizeCircleParameter(arcAngle1, v10, x1, y1);
			double t11 = NormalizeCircleParameter(arcAngle1, v11, x1, y1);

			intersectionPoint.Add(Add(c0, v00));
			intersectionPoint.Add(Add(c0, v01));
			relativePosition0.Add(t00);
			relativePosition0.Add(t01);
			relativePosition1.Add(t10);
			relativePosition1.Add(t11);
			return true;
		}

		/// <summary>
		/// Move segment according to given vector
		/// </summary>
		/// <param name="segment">Segment to be moved</param>
		/// <param name="displacement">Displacement vector</param>
		/// <param name="isIndependent">Segment is independent</param>
		public static void Move(ISegment3D segment, Vector3D displacement, bool isIndependent = true)
		{
			if (segment.SegmentType == SegmentType.Line)
			{
				ILineSegment3D line = segment as ILineSegment3D;
				if (line == null)
				{
					throw new InvalidCastException("Wrong segment data");
				}

				if (isIndependent)
				{
					Move(line.StartPoint, displacement);
				}

				Move(line.EndPoint, displacement);
			}
			else if (segment.SegmentType == SegmentType.CircularArc)
			{
				IArcSegment3D arc = segment as IArcSegment3D;
				if (arc == null)
				{
					throw new InvalidCastException("Wrong segment data");
				}

				if (isIndependent)
				{
					Move(arc.StartPoint, displacement);
				}

				Move(arc.IntermedPoint, displacement);
				Move(arc.EndPoint, displacement);
			}
			else
			{
				IArcSegment3D arc = segment as IArcSegment3D;
				if (arc == null)
				{
					throw new InvalidCastException("Wrong segment data");
				}

				if (isIndependent)
				{
					Move(arc.StartPoint, displacement);
				}

				Move(arc.IntermedPoint, displacement);
				Move(arc.EndPoint, displacement);
			}
		}

		/// <summary>
		/// Reverse orientation of a parabola
		/// </summary>
		/// <param name="arc">Input parabola</param>
		/// <returns>Reversed parabola</returns>
		public static IArcSegment3D ReverseParabola(IArcSegment3D arc)
		{
			IArcSegment3D reversed = new ParabolaSegment3D(arc.EndPoint, arc.IntermedPoint, arc.StartPoint);
			Segment3D seg = reversed as Segment3D;
			ICoordSystem coordSystem = null;
			(arc as Segment3D).LocalCoordinateSystem.CopyTo(ref coordSystem);
			seg.LocalCoordinateSystem = coordSystem;
			return reversed;
		}

		/// <summary>
		/// Reverse orientation of a given arc
		/// </summary>
		/// <param name="arc">Input arc</param>
		/// <returns>Reversed arc</returns>
		public static IArcSegment3D Reverse(IArcSegment3D arc)
		{
			IArcSegment3D reversed = new ArcSegment3D(arc.EndPoint, arc.IntermedPoint, arc.StartPoint);
			Segment3D seg = reversed as Segment3D;
			ICoordSystem coordSystem = null;
			(arc as Segment3D).LocalCoordinateSystem.CopyTo(ref coordSystem);
			seg.LocalCoordinateSystem = coordSystem;
			return reversed;
		}

		/// <summary>
		/// Reverse orientation of a given line
		/// </summary>
		/// <param name="line">Input line</param>
		/// <returns>Reversed line</returns>
		public static ILineSegment3D Reverse(ILineSegment3D line)
		{
			ILineSegment3D reversed = new LineSegment3D(line.EndPoint, line.StartPoint);
			Segment3D seg = reversed as Segment3D;
			ICoordSystem coordSystem = null;
			(line as Segment3D).LocalCoordinateSystem.CopyTo(ref coordSystem);
			seg.LocalCoordinateSystem = coordSystem;
			return reversed;
		}

		/// <summary>
		/// Reverse orientation of a given segment
		/// </summary>
		/// <param name="segment">Input segment</param>
		/// <returns>Reversed segment</returns>
		public static ISegment3D Reverse(ISegment3D segment)
		{
			if (segment == null)
			{
				return null;
			}

			if (segment.SegmentType == SegmentType.Line)
			{
				ILineSegment3D line = segment as ILineSegment3D;
				if (line == null)
				{
					throw new InvalidCastException("Wrong segment data");
				}

				return Reverse(line);
			}
			else if (segment.SegmentType == SegmentType.CircularArc)
			{
				IArcSegment3D arc = segment as IArcSegment3D;
				if (arc == null)
				{
					throw new InvalidCastException("Wrong segment data");
				}

				return Reverse(arc);
			}
			else if (segment.SegmentType == SegmentType.Parabola)
			{
				IArcSegment3D arc = segment as IArcSegment3D;
				if (arc == null)
				{
					throw new InvalidCastException("Wrong segment data");
				}

				return ReverseParabola(arc);
			}

			throw new NotSupportedException();
		}

		/// <summary>
		/// Transform a given segment using the matrix from LCS to GCS
		/// </summary>
		/// <param name="matrix">Matrix which is used to transform</param>
		/// <param name="segment">Segment which is to be transformed</param>
		/// <param name="isIndependent">Segment is independent</param>
		public static void TransformToGCS(IMatrix44 matrix, ISegment3D segment, bool isIndependent = true)
		{
			if (matrix == null || segment == null)
			{
				return;
			}

			if (segment.SegmentType == SegmentType.Line)
			{
				ILineSegment3D line = segment as ILineSegment3D;
				if (line == null)
				{
					throw new InvalidCastException("Wrong segment data");
				}

				if (isIndependent)
				{
					CopyValues(line.StartPoint, matrix.TransformToGCS(line.StartPoint));
				}

				CopyValues(line.EndPoint, matrix.TransformToGCS(line.EndPoint));
			}
			else if (segment.SegmentType == SegmentType.CircularArc)
			{
				IArcSegment3D arc = segment as IArcSegment3D;
				if (arc == null)
				{
					throw new InvalidCastException("Wrong segment data");
				}

				if (isIndependent)
				{
					CopyValues(arc.StartPoint, matrix.TransformToGCS(arc.StartPoint));
				}

				CopyValues(arc.IntermedPoint, matrix.TransformToGCS(arc.IntermedPoint));
				CopyValues(arc.EndPoint, matrix.TransformToGCS(arc.EndPoint));
			}
			else
			{
				throw new NotSupportedException();
			}
		}

		/// <summary>
		/// Transform a given segment using the matrix from GCS to LCS
		/// </summary>
		/// <param name="matrix">Matrix which is used to transform</param>
		/// <param name="segment">Segment which is to be transformed</param>
		/// <param name="isIndependent">Segment is independent</param>
		public static void TransformToLCS(IMatrix44 matrix, ISegment3D segment, bool isIndependent = true)
		{
			if (matrix == null || segment == null)
			{
				return;
			}

			if (segment.SegmentType == SegmentType.Line)
			{
				ILineSegment3D line = segment as ILineSegment3D;
				if (line == null)
				{
					throw new InvalidCastException("Wrong segment data");
				}

				if (isIndependent)
				{
					CopyValues(line.StartPoint, matrix.TransformToLCS(line.StartPoint));
				}

				CopyValues(line.EndPoint, matrix.TransformToLCS(line.EndPoint));
			}
			else if (segment.SegmentType == SegmentType.CircularArc || segment.SegmentType == SegmentType.Parabola)
			{
				IArcSegment3D arc = segment as IArcSegment3D;
				if (arc == null)
				{
					throw new InvalidCastException("Wrong segment data");
				}

				if (isIndependent)
				{
					CopyValues(arc.StartPoint, matrix.TransformToLCS(arc.StartPoint));
				}

				CopyValues(arc.IntermedPoint, matrix.TransformToLCS(arc.IntermedPoint));
				CopyValues(arc.EndPoint, matrix.TransformToLCS(arc.EndPoint));
			}
			else
			{
				throw new NotSupportedException();
			}
		}

		/// <summary>
		/// Check whether Arc Segment is Linear or not
		/// </summary>
		/// <param name="arcSegment">Segment Arc</param>
		/// <returns>True if linear</returns>
		public static bool IsLinear(IArcSegment3D arcSegment)
		{
			if (arcSegment == null)
			{
				return false;
			}

			Vector3D normal = GeomOperation.GetNormal(arcSegment, 1e-9);
			return normal.Magnitude.IsZero();
		}

		/////// <summary>
		/////// Prepare a list of segments from a arc segment based on given pattern
		/////// A pattern specifies the length of arc segments that make up the linetype. 
		/////// A positive decimal number specifies a pen-down (dash) segment of that length. 
		/////// A negative decimal number specifies a pen-up (space) segment of that length
		/////// </summary>
		/////// <param name="arc">Input value as an ArcSegment</param>
		/////// <param name="normal">Normal to the plane of arc</param>
		/////// <param name="pattern">Pattern defines the type of line</param>
		/////// <param name="beginOffset">Offset for the first segment</param>
		/////// <param name="index">Last index of pattern which is used</param>
		/////// <param name="newPolyline">New polyline</param>
		/////// <returns>returns balance length in the curve after the last point</returns>
		////public static double GetDividedSegments(IArcSegment3D arc, Vector3D normal, double[] pattern, double beginOffset, ref int index, IPolyLine3D newPolyline)
		////{
		////    double arcLength = GeomOperation.GetLength(arc);
		////    Vector3D direction = Subtract(arc.EndPoint, arc.StartPoint);
		////    IPoint3D startPoint = arc.StartPoint;
		////    double partLength = beginOffset;
		////    IPoint3D center = new Point3D();
		////    GetCentrePoint(arc, ref center);
		////    if (beginOffset.IsGreater(0, MathConstants.ZeroGeneral))
		////    {
		////        GetPointOnArc(center, arc.StartPoint, partLength, normal, ref startPoint);
		////    }

		////    while (partLength < arcLength)
		////    {
		////        double value = GetValue(pattern, ref index);
		////        partLength += Math.Abs(value);

		////        // Create new point with distance to start point is partLength
		////        IPoint3D endPoint = new Point3D();
		////        GetPointOnArc(center, arc.StartPoint, partLength, normal, ref endPoint);
		////        if (value.IsGreater(0, MathConstants.ZeroGeneral))
		////        {
		////            IPoint3D midPoint = new Point3D();
		////            GetPointOnArc(center, arc.StartPoint, (partLength - value * 0.5), normal, ref midPoint);
		////            double length = GetLength(new ArcSegment3D(arc.StartPoint, midPoint, endPoint));
		////            if (length.IsGreater(arcLength, MathConstants.ZeroGeneral))
		////            {
		////                endPoint = arc.EndPoint;
		////            }

		////            double magnitude = GeomOperation.Subtract(startPoint, endPoint).Magnitude;
		////            if (magnitude.IsGreater(0))
		////            {
		////                newPolyline.Add(new LineSegment3D(startPoint, endPoint));
		////                ////newPolyline.Add(new ArcSegment3D(startPoint, midPoint, endPoint));
		////            }
		////        }

		////        startPoint = endPoint;
		////    }

		////    return partLength - arcLength;
		////}

		/// <summary>
		/// Given arc is splitted into several segments with given length
		/// </summary>
		/// <param name="arc">Input value as an ArcSegment</param>
		/// <param name="normal">Normal to the plane of arc</param>
		/// <param name="distance">Length of each new segment</param>
		/// <param name="newPolyline">New polyline</param>
		public static void GetDividedSegments(IArcSegment3D arc, Vector3D normal, double distance, IPolyLine3D newPolyline)
		{
			double arcLength = GeomOperation.GetLength(arc);
			int divisionCount = (int)(arcLength / distance);
			if (divisionCount == 0)
			{
				return;
			}

			// TODO: Remove parameter "Vector3D normal"
			normal = GetNormal(arc, 1e-9).Normalize;

			if (IsCollinear(arc))
			{
				ILineSegment3D line = new LineSegment3D(arc.StartPoint, arc.EndPoint);
				GetDividedSegments(line, distance, newPolyline);
				return;
			}

			double newDistance = arcLength / divisionCount;
			Vector3D direction = Subtract(arc.EndPoint, arc.StartPoint);
			IPoint3D startPoint = arc.StartPoint;
			double partLength = 0;
			IPoint3D center = new Point3D();
			GetCentrePoint(arc, ref center);
			GetPointOnArc(center, arc.StartPoint, partLength, normal, ref startPoint);

			while (partLength < arcLength)
			{
				partLength += newDistance;

				// Create new point with distance to start point is partLength
				IPoint3D endPoint = new Point3D();
				GetPointOnArc(center, arc.StartPoint, partLength, normal, ref endPoint);
				IPoint3D midPoint = new Point3D();
				GetPointOnArc(center, arc.StartPoint, (partLength - newDistance * 0.5), normal, ref midPoint);
				double length = GetLength(new ArcSegment3D(arc.StartPoint, midPoint, endPoint));
				if (length.IsGreater(arcLength, MathConstants.ZeroGeneral))
				{
					endPoint = arc.EndPoint;
				}

				double magnitude = GeomOperation.Subtract(startPoint, endPoint).Magnitude;
				if (magnitude.IsGreater(0))
				{
					newPolyline.Add(new LineSegment3D(startPoint, endPoint));
					////newPolyline.Add(new ArcSegment3D(startPoint, midPoint, endPoint));
				}

				startPoint = endPoint;
			}
		}

		/////// <summary>
		/////// Prepare a list of segments from a line segment based on given pattern
		/////// A pattern specifies the length of segments that make up the linetype. 
		/////// A positive decimal number specifies a pen-down (dash) segment of that length. 
		/////// A negative decimal number specifies a pen-up (space) segment of that length
		/////// </summary>
		/////// <param name="line">Input value as a LineSegment</param>
		/////// <param name="pattern">Pattern defines the type of line</param>
		/////// <param name="beginOffset">Offset for the first segment</param>
		/////// <param name="index">Last index of pattern which is used</param>
		/////// <param name="newPolyline">New polyline</param>
		/////// <returns>returns balance length in the curve after the last point</returns>
		////public static double GetDividedSegments(ILineSegment3D line, double[] pattern, double beginOffset, ref int index, IPolyLine3D newPolyline)
		////{
		////    double segmentLength = GeomOperation.GetLength(line);

		////    Vector3D direction = Subtract(line.EndPoint, line.StartPoint);
		////    IPoint3D startPoint = line.StartPoint;
		////    double partLength = beginOffset;
		////    if (beginOffset.IsGreater(0, MathConstants.ZeroGeneral))
		////    {
		////        Vector3D newDirection = direction * (partLength / segmentLength);
		////        startPoint = Add(line.StartPoint, newDirection);
		////    }

		////    while (partLength < segmentLength)
		////    {
		////        double value = GetValue(pattern, ref index);
		////        partLength += Math.Abs(value);

		////        // Create new point with distance to start point is partLength
		////        Vector3D newDirection = direction * (partLength / segmentLength);
		////        IPoint3D endPoint = Add(line.StartPoint, newDirection);
		////        if (value.IsGreater(0, MathConstants.ZeroGeneral))
		////        {
		////            if (GeomOperation.Distance(line.StartPoint, endPoint).IsGreater(segmentLength, MathConstants.ZeroGeneral))
		////            {
		////                endPoint = line.EndPoint;
		////            }

		////            double magnitude = GeomOperation.Subtract(startPoint, endPoint).Magnitude;
		////            if (magnitude.IsGreater(0))
		////            {
		////                newPolyline.Add(new LineSegment3D(startPoint, endPoint));
		////            }
		////        }

		////        startPoint = endPoint;
		////    }

		////    return partLength - segmentLength;
		////}

		/// <summary>
		/// Given ILineSegment3D is splitted into several segments with given length
		/// </summary>
		/// <param name="line">Input value as an ILineSegment3D</param>
		/// <param name="distance">Length of each new segment</param>
		/// <param name="newPolyline">New polyline</param>
		public static void GetDividedSegments(ILineSegment3D line, double distance, IPolyLine3D newPolyline)
		{
			double segmentLength = GeomOperation.GetLength(line);
			int divisionCount = (int)(segmentLength / distance);
			divisionCount = Math.Max(divisionCount, 1);

			double newDistance = segmentLength / divisionCount;
			Vector3D direction = Subtract(line.EndPoint, line.StartPoint);
			IPoint3D startPoint = line.StartPoint;
			double partLength = 0;
			Vector3D newDirection = direction * (partLength / segmentLength);
			startPoint = Add(line.StartPoint, newDirection);

			while (partLength < segmentLength)
			{
				partLength += newDistance;

				// Create new point with distance to start point is partLength
				newDirection = direction * (partLength / segmentLength);
				IPoint3D endPoint = Add(line.StartPoint, newDirection);
				if (GeomOperation.Distance(line.StartPoint, endPoint).IsGreater(segmentLength, MathConstants.ZeroGeneral))
				{
					endPoint = line.EndPoint;
				}

				double magnitude = GeomOperation.Subtract(startPoint, endPoint).Magnitude;
				if (magnitude.IsGreater(0))
				{
					newPolyline.Add(new LineSegment3D(startPoint, endPoint));
				}

				startPoint = endPoint;
			}
		}

		/// <summary>
		/// Divide the given segment into given number of segments
		/// </summary>
		/// <param name="segment">Original segment</param>
		/// <param name="normal">Normal to the plane of segment</param>
		/// <param name="length">Length of each new segment</param>
		/// <param name="newPolyline">New polygon which contains the splitted segments</param>
		public static void GetDividedSegments(ISegment3D segment, Vector3D normal, double length, IPolyLine3D newPolyline)
		{
			if (segment.SegmentType == SegmentType.Line)
			{
				ILineSegment3D line = segment as ILineSegment3D;
				if (line == null)
				{
					throw new InvalidCastException("Wrong segment data");
				}

				GetDividedSegments(line, length, newPolyline);
			}
			else if (segment.SegmentType == SegmentType.CircularArc)
			{
				IArcSegment3D arc = segment as IArcSegment3D;
				if (arc == null)
				{
					throw new InvalidCastException("Wrong segment data");
				}

				GetDividedSegments(arc, normal, length, newPolyline);
			}
			else
			{
				throw new NotSupportedException();
			}
		}

		/////// <summary>
		/////// Prepare a list of segments from a segment based on given pattern
		/////// A pattern specifies the length of segments that make up the linetype. 
		/////// A positive decimal number specifies a pen-down (dash) segment of that length. 
		/////// A negative decimal number specifies a pen-up (space) segment of that length
		/////// </summary>
		/////// <param name="segment">Input value as a segment</param>
		/////// <param name="normal">Normal to the plane of segment</param>
		/////// <param name="pattern">Pattern defines the type of line</param>
		/////// <param name="beginOffset">Offset for the first segment</param>
		/////// <param name="index">Last index of pattern which is used</param>
		/////// <param name="newPolyline">New polyline</param>
		/////// <returns>returns balance length in the curve after the last point</returns>
		////public static double GetDividedSegments(ISegment3D segment, Vector3D normal, double[] pattern, double beginOffset, ref int index, IPolyLine3D newPolyline)
		////{
		////    if (segment.SegmentType == SegmentType.Line)
		////    {
		////        ILineSegment3D line = segment as ILineSegment3D;
		////        if (line == null)
		////        {
		////            throw new InvalidCastException("Wrong segment data");
		////        }

		////        return GetDividedSegments(line, pattern, beginOffset, ref index, newPolyline);
		////    }
		////    else if (segment.SegmentType == SegmentType.CircularArc)
		////    {
		////        IArcSegment3D arc = segment as IArcSegment3D;
		////        if (arc == null)
		////        {
		////            throw new InvalidCastException("Wrong segment data");
		////        }

		////        return GetDividedSegments(arc, normal, pattern, beginOffset, ref index, newPolyline);
		////    }

		////    throw new NotSupportedException();
		////}

		/// <summary>
		/// GetIntersectionOfLines - Get the intersection point of two Lines.
		/// </summary>
		/// <param name="line1">Line1 to be compared</param>
		/// <param name="line2">Line2 to be compared</param>
		/// <param name="relativePosition0">Relative Position for Line1</param>
		/// <param name="relativePosition1">Relative Position for Line2</param>
		/// <param name="lineParallel">check Lines are parallel</param>
		/// <param name="precision">precision</param>
		/// <returns>return true if Lines are intersect</returns>
		public static int GetIntersectionOfLines(ILineSegment3D line1, ILineSegment3D line2, ref double relativePosition0, ref double relativePosition1, ref bool lineParallel, double precision = MathConstants.ZeroMaximum)
		{
			int count = 0;
			double valueZero = 0.0;
			double valueOne = 1.0;
			if (Subtract(line1.StartPoint, line2.StartPoint).Magnitude.Equals(0.0))
			{
				count++;
				relativePosition0 = valueZero;
				relativePosition1 = valueZero;
			}
			else if (Subtract(line1.StartPoint, line2.EndPoint).Magnitude.Equals(0.0))
			{
				count++;
				relativePosition0 = valueZero;
				relativePosition1 = valueOne;
			}

			if (Subtract(line1.EndPoint, line2.StartPoint).Magnitude.Equals(0.0))
			{
				count++;
				relativePosition0 = valueOne;
				relativePosition1 = valueZero;
			}
			else if (Subtract(line1.EndPoint, line2.EndPoint).Magnitude.Equals(0.0))
			{
				count++;
				relativePosition0 = valueOne;
				relativePosition1 = valueOne;
			}

			Vector3D vector0 = Subtract(line1.EndPoint, line1.StartPoint);
			Vector3D vector1 = Subtract(line2.EndPoint, line2.StartPoint);

			Vector3D normalVector = vector0 * vector1;
			lineParallel = normalVector.Magnitude.IsEqual(0.0);
			if (!lineParallel)
			{
				normalVector = normalVector.Normalize;
			}
			else
			{
				return 0;
			}

			Vector3D normal1 = (vector1 * normalVector).Normalize;
			double nom0 = vector0 | normal1;
			if (nom0.Equals(0.0))
			{
				return count;
			}

			double relativeValue0 = (Subtract(line2.StartPoint, line1.StartPoint) | normal1) / nom0;

			Vector3D normalVector0 = (vector0 * normalVector).Normalize;
			double nom1 = vector1 | normalVector0;
			if (nom1.Equals(0.0))
			{
				return count;
			}

			double relativeValue1 = (Subtract(line1.StartPoint, line2.StartPoint) | normalVector0) / nom1;
			IPoint3D relativePoint0 = Add(line1.StartPoint, (relativeValue0 * vector0));
			IPoint3D relativePoint1 = Add(line2.StartPoint, (relativeValue1 * vector1));
			if (Subtract(relativePoint0, relativePoint1).Magnitude.IsGreater(precision))
			{
				return 0;
			}

			count++;
			relativePosition0 = relativeValue0;
			relativePosition1 = relativeValue1;
			return count;
		}

		/// <summary>
		/// Create and return a matrix based on given two points
		/// The matrix represents a coordinate system. 
		/// </summary>
		/// <param name="arc">Segment Arc</param>
		/// <returns>The matrix which represents the co-ordinate system</returns>
		public static Matrix44 GetLCSMatrix(IArcSegment3D arc)
		{
			IPoint3D center = null;
			GeomOperation.GetCentrePoint(arc, ref center);
			//Vector3D localX = GeomOperation.Subtract(arc.StartPoint, center).Normalize;
			//Vector3D localZ = GeomOperation.GetNormal(arc).Normalize;
			Vector3D localX = GeomOperation.Subtract(arc.StartPoint, center);
			Vector3D localZ = GeomOperation.GetNormal(arc, 1e-9);
			if (GeomOperation.IsCollinear(localZ, localX, MathConstants.ZeroGeneral))
			{
				throw new InvalidOperationException("Wrong arc data");
			}

			//Vector3D localY = (localZ * localX).Normalize;
			//Matrix44 matrix = new Matrix44(arc.StartPoint, localX, localY, localZ);
			Vector3D localY = (localZ * localX).Normalize;
			Matrix44 matrix = new Matrix44(arc.StartPoint, localX.Normalize, localY, localZ.Normalize);
			return matrix;
		}

		/// <summary>
		/// Calculate the Length of segment
		/// </summary>
		/// <param name="segment">segment</param>
		/// <returns>Length of given segment</returns>
		public static double GetLength(ISegment3D segment)
		{
			if (segment.SegmentType == SegmentType.Line)
			{
				Vector3D direction = Subtract(segment.EndPoint, segment.StartPoint);
				return ~direction;
			}
			else if (segment.SegmentType == SegmentType.CircularArc)
			{
				IArcSegment3D arc = segment as IArcSegment3D;
				if (arc == null)
				{
					throw new InvalidCastException("Wrong segment data");
				}

				if (IsCollinear(arc))
				{
					Vector3D direction = Subtract(segment.EndPoint, segment.StartPoint);
					return ~direction;
				}

				//To je neštěstí, tady to zase nepočítá
				double angle = GetArcAngle(arc);
				double radius = GetRadius(arc);
				if (radius.IsLesserOrEqual(0, 1e-6))
				{
					return (~Subtract(arc.IntermedPoint, arc.StartPoint) + ~Subtract(arc.EndPoint, arc.IntermedPoint));
				}

				return angle * radius;
			}
			else if (segment.SegmentType == SegmentType.Parabola)
			{
				// CIH - proc je tady ta transformace ?
				// IArcSegment3D arcSegment = segment.CloneSegment() as IArcSegment3D;
				// if (arcSegment == null)
				// {
				//	throw new InvalidCastException("Wrong segment data");
				// }

				// Matrix44 mat = GeomOperation.GetLCSMatrix(arcSegment);
				// GeomOperation.TransformToLCS(mat, arcSegment);
				// double parabolaLength = 0;
				// CalculateParabolaLength(arcSegment, ref parabolaLength);
				// return parabolaLength;

				IArcSegment3D arcSegment = segment as IArcSegment3D;
				if (arcSegment == null)
				{
					throw new InvalidCastException("Wrong segment data");
				}

				double parabolaLength = 0;
				CalculateParabolaLength(arcSegment, ref parabolaLength);
				return parabolaLength;
			}

			throw new NotSupportedException();
		}

		/// <summary>
		/// Get nearest point on arc
		/// NOTE: THIS IS NOT AN ACCURATE METHOD.
		/// </summary>
		/// <param name="arc">Input arc</param>
		/// <param name="normal">Normal to the arc</param>
		/// <param name="point">Input point</param>
		/// <returns>The point on arc</returns>
		public static IPoint3D GetNearestPoint(IArcSegment3D arc, Vector3D normal, IPoint3D point)
		{
			if (IsCollinear(arc))
			{
				return GetNearestPoint(new LineSegment3D(arc.StartPoint, arc.EndPoint), point);
			}

			double distance = GetLength(arc) / 15;
			IPolyLine3D newPolyline = new PolyLine3D();
			GetDividedSegments(arc, normal, distance, newPolyline);
			return GetNearestPoint(newPolyline, point);
		}

		/// <summary>
		/// Get nearest point on line
		/// </summary>
		/// <param name="line">Input line</param>
		/// <param name="point">Input point</param>
		/// <returns>The point on line</returns>
		public static IPoint3D GetNearestPoint(ILineSegment3D line, IPoint3D point)
		{
			Vector3D vector1 = Subtract(line.EndPoint, line.StartPoint);
			Vector3D vector2 = Subtract(line.StartPoint, point);
			Vector3D vector = (vector1 * vector2) * (vector1 / (vector1 | vector1));
			IPoint3D nearestPoint = Clone(point);
			Move(nearestPoint, vector);
			double relativePosition = 0;
			if (IsPointOnSegment(line, nearestPoint, ref relativePosition) == false)
			{
				double distanceToStart = Subtract(point, line.StartPoint).Magnitude;
				double distanceToEnd = Subtract(point, line.EndPoint).Magnitude;
				if (distanceToStart.IsGreater(distanceToEnd))
				{
					nearestPoint = Clone(line.EndPoint);
				}
				else
				{
					nearestPoint = Clone(line.StartPoint);
				}
			}

			return nearestPoint;
		}

		/// <summary>
		/// Finds the perpendicular to the segment passing specified point.
		/// </summary>
		/// <param name="segment">The segment.</param>
		/// <param name="point">The point.</param>
		/// <param name="relativePosition">The relative position of the nearest intersection of perpendicular and segment from the begin of segment.</param>
		/// <returns>True, if point is on the segment, false otherwise.</returns>
		public static bool GetPerpendicuralPoint(ISegment3D segment, IPoint3D point, ref double relativePosition)
		{
			switch (segment.SegmentType)
			{
				case SegmentType.Line:
					relativePosition = GetPerpendicularPointToLine(segment.StartPoint, segment.EndPoint, point);
					break;

				case SegmentType.CircularArc:
					IPoint3D centre = new Point3D();
					if (!GetCentrePoint(segment as IArcSegment3D, ref centre))
					{
						relativePosition = GetPerpendicularPointToLine(segment.StartPoint, segment.EndPoint, point);
					}
					else
					{
						var u = Subtract(segment.StartPoint, centre);
						var u1 = Subtract(segment.EndPoint, centre);
						var n = (u * u1).Normalize;
						if (n.Magnitude.IsZero())
						{
							u1 = Subtract((segment as IArcSegment3D).IntermedPoint, centre);
							n = (u * u1).Normalize;
						}

						var d = n | n;
						var dd = (GeomOperation.Subtract(centre, point) | n) / d;
						var point1 = GeomOperation.Add(point, dd * n);
						var v = Subtract(point1, centre);
						if (v.Magnitude.IsZero())
						{
							return false;
						}

						var angle = GetAngle(u, v);
						var totalAngle = GetAngle(u, Subtract(segment.EndPoint, centre));
						relativePosition = angle / totalAngle;
					}

					break;

				case SegmentType.Parabola:
					IList<double> relativeCollection = new List<double>();
					GeomOperation.GetQuadraticSplineRelativePosition(segment as IArcSegment3D, Math.PI / 36.0, ref relativeCollection);
					var count = relativeCollection.Count;
					IPoint3D endPt = null;
					IPoint3D begPt = segment.StartPoint;
					var l = 0d;
					double rel;
					relativePosition = -1;
					for (var i = 1; i < count; ++i)
					{
						if (GeomOperation.GetPointOnSegment(segment, relativeCollection[i], ref endPt))
						{
							rel = GetPerpendicularPointToLine(begPt, endPt, point);
							if (relativePosition >= 0 && relativePosition <= 1)
							{
								var length = GeomOperation.GetLength(segment);
								relativePosition = rel + (l / length);
								//// TODO save all this points and find the nearest to the point...
								return true;
							}

							l += Subtract(endPt, begPt).Magnitude;
							begPt = endPt;
						}
					}

					endPt = segment.EndPoint;
					rel = GetPerpendicularPointToLine(begPt, endPt, point);
					if (relativePosition >= 0 && relativePosition <= 1)
					{
						var length = GeomOperation.GetLength(segment);
						relativePosition = rel + (l / length);
					}

					break;

				default:
					throw new NotImplementedException();
			}

			return relativePosition >= 0 && relativePosition <= 1;
		}

		/// <summary>
		/// Gets previous segment in the polyline
		/// </summary>
		/// <param name="polyline">Polyline</param>
		/// <param name="segmentInx">Index of thesegment</param>
		/// <returns>Previous segment</returns>
		public static ISegment3D GetPreviousSegment(IPolyLine3D polyline, int segmentInx)
		{
			int outlineSegCount = polyline.Count;
			Debug.Assert(outlineSegCount > 1);
			if (segmentInx == 0)
			{
				return polyline[outlineSegCount - 1];
			}

			return polyline[segmentInx - 1];
		}

		/// <summary>
		/// Gets the index of the previous segment in the polyline
		/// </summary>
		/// <param name="polyline">Polyline</param>
		/// <param name="segmentInx">Index of the segment</param>
		/// <returns>The index of the previous</returns>
		public static int GetPreviousSegmentIndex(IPolyLine3D polyline, int segmentInx)
		{
			int outlineSegCount = polyline.Count;
			Debug.Assert(outlineSegCount > 1);
			if (segmentInx == 0)
			{
				return (outlineSegCount - 1);
			}

			return (segmentInx - 1);
		}

		/// <summary>
		/// Gets the next segment in the polyline
		/// </summary>
		/// <param name="polyline">Polyline</param>
		/// <param name="segmentInx">Index of the segment</param>
		/// <returns>Next segment</returns>
		public static ISegment3D GetNextSegment(IPolyLine3D polyline, int segmentInx)
		{
			int outlineSegCount = polyline.Count;
			Debug.Assert(outlineSegCount > 1);
			if (segmentInx == (outlineSegCount - 1))
			{
				return polyline[0];
			}

			return polyline[segmentInx + 1];
		}

		/// <summary>
		/// Gets the index of the next segment in the polyline
		/// </summary>
		/// <param name="polyline">Polyline</param>
		/// <param name="segmentInx">Index of the segment</param>
		/// <returns>The index of the next segment</returns>
		public static int GetNextSegmentIndex(IPolyLine3D polyline, int segmentInx)
		{
			int outlineSegCount = polyline.Count;
			Debug.Assert(outlineSegCount > 1);
			if (segmentInx == (outlineSegCount - 1))
			{
				return 0;
			}

			return (segmentInx + 1);
		}

		/// <summary>
		/// Calculate Nearest point on Segment.
		/// </summary>
		/// <param name="pointOnSegment">Point On Segment</param>
		/// <param name="segment">Segment</param>
		/// <param name="resultCollection">Result Collection</param>
		/// <param name="relativePoint">calculated the relative point</param>
		/// <returns>relative position</returns>
		private static double GetNearestPoint(IPoint3D pointOnSegment, ISegment3D segment, IList<double> resultCollection, ref IPoint3D relativePoint)
		{
			// TODO: Use IEnumerator instead of IList
			double dist = 1e100, relativeDistance = 0.0, resultPosition = 0.0;

			for (int i = 0; i < resultCollection.Count; i++)
			{
				double result = resultCollection[i];
				GetPointOnSegment(segment, result, ref relativePoint);
				relativeDistance = Subtract(relativePoint, pointOnSegment).Magnitude;
				if (relativeDistance.IsLesser(dist) || (relativeDistance.IsEqual(dist) && result.IsGreaterOrEqual(0) && result.IsLesserOrEqual(1)))
				{
					dist = relativeDistance;
					resultPosition = result;
				}
			}

			return resultPosition;
		}

		/// <summary>
		/// calculates the relative position for Line segment based on a given point
		/// </summary>
		/// <param name="segment">Segment</param>
		/// <param name="relativePoint">Point on segment</param>
		/// <param name="unifiedLength">Unified segment Length</param>
		/// <returns>relative position of segment</returns>
		private static double GetRelativePosition(ISegment3D segment, IPoint3D relativePoint, double unifiedLength)
		{
			IPoint3D offsetPoint = new Point3D();
			GetPointOnSegment(segment, unifiedLength, ref offsetPoint);

			Vector3D vectorA = new Vector3D();
			GetTangentOnSegment(segment, unifiedLength, ref vectorA);
			Vector3D vectorCL = Subtract(relativePoint, offsetPoint);
			return vectorCL | vectorA.Normalize;
		}

		/// <summary>
		/// Check whether Relaive Position is Valid.
		/// </summary>
		/// <param name="relaiveValue">Relative Position</param>
		/// <returns>return true if relative position is valid</returns>
		private static bool IsValidRelativePosition(double relaiveValue)
		{
			if (relaiveValue.IsGreaterOrEqual(0.0) && relaiveValue.IsLesserOrEqual(1.0))
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Check whether relative position lies on segment.
		/// </summary>
		/// <param name="relativePosition1">relative position of segment1</param>
		/// <param name="relativePosition2">relative position of segment2</param>
		/// <returns>true if position is on segment</returns>
		private static bool IsPositionOnSegment(double relativePosition1, double relativePosition2)
		{
			if (relativePosition1.IsGreater(0) && relativePosition1.IsLesser(1))
			{
				if (relativePosition2.IsGreater(0) && relativePosition2.IsLesser(1))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// NormalizeCircleParameter - return the param value of circle based on the given angle.
		/// </summary>
		/// <param name="arcAngle">Angle of Arc</param>
		/// <param name="vectorI">Arc Normal Vector</param>
		/// <param name="vectorX">Arc LocalX Vector</param>
		/// <param name="vectorY">Arc LocalY Vecotr</param>
		/// <returns>returns the relative offset of arc</returns>
		private static double NormalizeCircleParameter(double arcAngle, Vector3D vectorI, Vector3D vectorX, Vector3D vectorY)
		{
			double relativeOffset = vectorI.Normalize | vectorX.Normalize;
			if (relativeOffset.IsGreater(1.0, MathConstants.ZeroGeneral))
			{
				relativeOffset = 1.0;
			}
			else if (relativeOffset.IsLesser(-1.0, MathConstants.ZeroGeneral))
			{
				relativeOffset = -1.0;
			}

			relativeOffset = Math.Acos(relativeOffset);

			if ((vectorI | vectorY) < 0.0)
			{
				double t1 = (2.0 * Math.PI) - relativeOffset;

				relativeOffset = ((t1 > arcAngle) && (relativeOffset < Math.Abs(t1 - arcAngle))) ? -relativeOffset : t1;
			}

			if (!arcAngle.IsZero(MathConstants.ZeroGeneral))
			{
				relativeOffset /= arcAngle;
			}

			return relativeOffset;
		}

		/// <summary>
		/// Calculate the length of parabola.
		/// </summary>
		/// <param name="parabolicArc">Segment Parabola</param>
		/// <param name="parabolaLength">length of parabola</param>
		/// <returns>true if given parabola segment is valid to calculate Length</returns>
		private static bool CalculateParabolaLength(IArcSegment3D parabolicArc, ref double parabolaLength)
		{
			ParabolaProperty parabolaProperty = (parabolicArc as ParabolaSegment3D).Property;
			if (parabolaProperty == null)
			{
				return false;
			}

			parabolaLength = parabolaProperty.ParabolaLength;
			return true;
		}

		/// <summary>
		/// Save the Relative position of a segment
		/// </summary>
		/// <param name="relativePosition">relative position of segment</param>
		/// <param name="trials">Maximum number of trials allowed</param>
		/// <param name="resultCollection">Result Collection</param>
		private static void SaveResult(double relativePosition, int trials, IList<double> resultCollection)
		{
			// TODO: Use ICollection instead of IList
			double minimumOffset = -1e100, maximumOffset = 1e100;

			if (resultCollection.Count >= trials)
			{
				return;
			}

			bool save = relativePosition >= minimumOffset && relativePosition <= maximumOffset;

			for (int i = 0; (i < resultCollection.Count) && save; i++)
			{
				save = Math.Abs(resultCollection[i] - relativePosition) > 10 * MathConstants.ZeroGeneral;
			}

			if (save)
			{
				resultCollection.Add(relativePosition);
			}
		}

		/// <summary>
		/// Determines whether relative position is on segment.
		/// </summary>
		/// <param name="relativePosition">Relative position of segment</param>
		/// <param name="unifiedLength">Unified segment Length</param>
		/// <param name="toleranceLevel">Tolerance Level</param>
		/// <returns>true if relative position lies on segment</returns>
		private static bool IsRelativePositionOnSegment(double relativePosition, double unifiedLength, double toleranceLevel)
		{
			return (relativePosition * unifiedLength).IsGreater(-toleranceLevel) && (unifiedLength * (relativePosition - 1.0)).IsLesser(toleranceLevel);
		}

		/// <summary>
		/// Calculate the relative position of point on segment.
		/// </summary>
		/// <param name="arcSegment">segment used to find the relative values</param>
		/// <param name="relativePoint">used to calculate the relative position</param>
		/// <param name="circleGeometry">Is Circle geometry or not</param>
		/// <returns>return relative position collection</returns>
		private static IList<double> FindSolution(ISegment3D arcSegment, IPoint3D relativePoint, bool circleGeometry)
		{
			IList<double> resultCollection = new List<double>();

			double maximumStep = 1e100, startPoint = 0.0, endPoint = 1.0;
			int trials = 5, maximumIterations = 100;
			double actt = 0.0;
			double actValue;
			double dt = 1e-5;
			int maxIterPerTrial = (maximumIterations / trials) + 1;

			int trial = 0;

			double bestParam = 0.0;
			double bestVal = 1e100;

			for (int iter = 0, trialIter = 0; iter < maximumIterations; trialIter++, iter++)
			{
				bool saved = false;
				actValue = GetRelativePosition(arcSegment, relativePoint, actt);

				if (Math.Abs(actValue) < bestVal)
				{
					bestVal = Math.Abs(actValue);
					bestParam = actt;
				}
				else
				{
					saved = bestVal < MathConstants.ZeroGeneral;
				}

				if (saved || (trialIter > maxIterPerTrial))
				{
					if (saved)
					{
						SaveResult(bestParam, trials, resultCollection);
					}

					if (resultCollection.Count >= trials)
					{
						break;
					}

					trial++;
					if (trial >= trials)
					{
						break;
					}

					actt = startPoint + trial * (endPoint - startPoint) / (double)(trials - 1);

					trialIter = 0;
					bestParam = bestVal = 1e100;
				}
				else
				{
					double dif_value_dt = GetRelativePosition(arcSegment, relativePoint, actt + dt) - actValue;

					double step = (dif_value_dt == 0) ? 1 : -(dt / dif_value_dt) * actValue;

					if (step > maximumStep)
					{
						step = maximumStep;
					}

					if (step < -maximumStep)
					{
						step = -maximumStep;
					}

					actt += step;
					if (circleGeometry)
					{
						actt = actt - Math.Floor(actt); // frac(Act_t);
					}
				}
			}

			return resultCollection;
		}

		private static double GetPerpendicularPointToLine(IPoint3D a, IPoint3D b, IPoint3D p)
		{
			var u = Subtract(b, a);
			var v = Subtract(p, a);
			var d = u.Normalize | v;
			return d / u.Magnitude;
		}
	}
}