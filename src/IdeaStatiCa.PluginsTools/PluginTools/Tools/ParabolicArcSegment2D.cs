using System;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;
using CI.Mathematics;

namespace CI.Geometry2D
{
	/// <summary>
	/// Creates a parabolic arc between two points in a PolyLine2D.
	/// </summary>
	[Obfuscation(Feature = "renaming")]
	public class ParabolicArcSegment2D : Segment2D
	{
		#region Fields

		// y = ax^2 + bx + c
		private double a, b, c;

		private Point? startCache;
		private Point point;
		private double angle;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the ParabolicArcSegment2D class.
		/// </summary>
		public ParabolicArcSegment2D()
		{
		}

		/// <summary>
		/// Initializes a new instance of the ParabolicArcSegment2D class that has the specified end point and any other point.
		/// </summary>
		/// <param name="endPoint">The end point of the arc.</param>
		/// <param name="point">A point of the parabola.</param>
		public ParabolicArcSegment2D(Point endPoint, Point point)
		{
			base.EndPoint = endPoint;
			Point = point;
		}

		/// <summary>
		/// Copies the source into a new instance of <c>ParabolicArcSegment2D</c>.
		/// </summary>
		/// <param name="source">The source.</param>
		public ParabolicArcSegment2D(ParabolicArcSegment2D source)
		{
			base.EndPoint = source.EndPoint;
			Point = source.Point;
		}

		#endregion Constructors

		#region Properties

		/// <summary>
		/// Gets or sets the endpoint of the segment.
		/// </summary>
		[XmlIgnore]
		public new Point EndPoint
		{
			get
			{
				return base.EndPoint;
			}

			set
			{
				base.EndPoint = value;
				Reset();
			}
		}

		/// <summary>
		/// Gets or sets the any point of parabola.
		/// </summary>
		public Point Point
		{
			get
			{
				return this.point;
			}

			set
			{
				if (this.point != value)
				{
					this.point = value;
					Reset();
				}
			}
		}

		/// <summary>
		/// Gets or sets the angle between global x axis and local x axis of parabola.
		/// </summary>
		public double Angle
		{
			get
			{
				return this.angle;
			}

			set
			{
				if (this.angle != value)
				{
					this.angle = value;
					Reset();
				}

				throw new NotImplementedException("Not fully implemented");
			}
		}

		#endregion Properties

		#region Static public methods

		/// <summary>
		/// Creates a parabolic arc segment by end point and vertex point.
		/// </summary>
		/// <param name="vertex">The vertex point of parabola.</param>
		/// <param name="point">The any point of parabola (can be start point).</param>
		/// <param name="vertexIsEndPoint">True, if vertex is End point of segment; false, if point is end point of segment.</param>
		/// <returns>A new Parabolic arc segment.</returns>
		public static ParabolicArcSegment2D CreateByVertex(Point vertex, Point point, bool vertexIsEndPoint)
		{
			var d = point.X - vertex.X;
			var a = (point.Y - vertex.Y) / (d * d);
			var b = -2 * a * vertex.X;
			var c = vertex.Y + vertex.X * vertex.X * a;
			var x = (vertex.X + point.X) / 2;
			var y = a * x * x + b * x + c;
			if (vertexIsEndPoint)
			{
				return new ParabolicArcSegment2D(vertex, new Point(x, y));
			}
			else
			{
				return new ParabolicArcSegment2D(point, new Point(x, y));
			}
		}

		/// <summary>
		/// Creates a parabolic arc segment by given point p1 and tangent in this point and by given point p2.
		/// Point p2 is EndPoint also.
		/// </summary>
		/// <param name="p1">The first point of segment.</param>
		/// <param name="u">The direction of tangent in specified point p1.</param>
		/// <param name="p2">The end point of segment.</param>
		/// <returns>A new Parabolic arc segment.</returns>
		public static ParabolicArcSegment2D CreateByTangent(Point p1, Vector u, Point p2)
		{
			var d = u.Y / u.X;
			var a = (p1.Y - d * p1.X - p2.Y + d * p2.X) / (2 * p1.X * p2.X - p1.X * p1.X - p2.X * p2.X);
			var b = d - 2 * a * p1.X;
			var c = p1.Y - a * p1.X * p1.X - b * p1.X;
			var x = (p1.X + p2.X) / 2;
			var y = a * x * x + b * x + c;
			return new ParabolicArcSegment2D(p2, new Point(x, y));
		}

		/// <summary>
		/// Creates a parabolic arc segment by specified point p1 (end point) and by specified point p2 and tangent in this point.
		/// </summary>
		/// <param name="p1">The end point of segment.</param>
		/// <param name="p2">Any point of segment.</param>
		/// <param name="u">The direction of tangent in specified point p2.</param>
		/// <returns>A new Parabolic arc segment.</returns>
		public static ParabolicArcSegment2D CreateByTangent(Point p1, Point p2, Vector u)
		{
			var d = u.Y / u.X;
			var a = (p2.Y - d * p2.X - p1.Y + d * p1.X) / (2 * p2.X * p1.X - p2.X * p2.X - p1.X * p1.X);
			var b = d - 2 * a * p2.X;
			var c = p2.Y - a * p2.X * p2.X - b * p2.X;
			var x = (p2.X + p1.X) / 2;
			var y = a * x * x + b * x + c;
			return new ParabolicArcSegment2D(p1, new Point(x, y));
		}

		/// <summary>
		/// Creates a new instance of parabolic arc segment given two points and tangent (point t and vector u).
		/// </summary>
		/// <param name="p1">The first point of segment.</param>
		/// <param name="p2">The second point of segment.</param>
		/// <param name="t">The point on tangent.</param>
		/// <param name="u">The dorection of tangent.</param>
		/// <param name="endPointIsP2">True, if EndPoint of segment is given point p2; false, if EndPoint is point p1.</param>
		/// <returns>A new Parabolic arc segment.</returns>
		public static ParabolicArcSegment2D CreateByTangent(Point p1, Point p2, Point t, Vector u, bool endPointIsP2)
		{
			var d = u.Y / u.X;
			var a = (p1.Y - p2.Y + d * p2.X - d * p1.X) / (p1.X * p1.X - p2.X * p2.X + 2 * t.X * p2.X - 2 * t.X * p1.X);
			var b = d - 2 * a * t.X;
			var c = p1.Y - a * p1.X * p1.X - b * p1.X;
			var x = (p1.X + p2.X) / 2;
			var y = a * x * x + b * x + c;
			if (endPointIsP2)
			{
				return new ParabolicArcSegment2D(p2, new Point(x, y));
			}
			else
			{
				return new ParabolicArcSegment2D(p1, new Point(x, y));
			}
		}

		#endregion Static public methods

		#region Segment2D overrides

		/// <summary>
		/// Gets the length of a parabolic arc segment.
		/// </summary>
		/// <param name="start">The start point of parabolic arc segment.</param>
		/// <returns>The lenght of segment.</returns>
		public override double GetLength(ref Point start)
		{
			Refresh(ref start);
			if (a == 0)
			{
				return Point.Subtract(EndPoint, start).Length;
			}

			var l1 = EvaluateLengthIntegral(start.X);
			var l2 = EvaluateLengthIntegral(EndPoint.X);
			return Math.Abs(l1 - l2);
		}

		/// <summary>
		/// Gets the rectangular boundary of a parabolic arc segment.
		/// </summary>
		/// <param name="start">The start point of the segment.</param>
		/// <returns>The boundary of segment.</returns>
		public override Rect GetBounds(ref Point start)
		{
			Refresh(ref start);
			var r = new Rect(start, EndPoint);
			var xt = -b / (2 * a);
			if (xt <= Math.Min(start.X, EndPoint.X) || xt >= Math.Max(start.X, EndPoint.X))
			{
				return r;
			}

			var yt = (a * xt * xt) + (b * xt) + c;
			r.Union(new Point(xt, yt));
			return r;
		}

		/// <summary>
		/// Gets a point on this segment defined by relative position.
		/// This method doesn't checks, if relative position is in range (0, 1).
		/// </summary>
		/// <param name="start">The start point of the segment.</param>
		/// <param name="relPosition">The relative position on the segment.</param>
		/// <returns>A point on the segment.</returns>
		public override Point GetPointOnSegment(ref Point start, double relPosition)
		{
			Refresh(ref start);
			if (a == 0)
			{
				Vector v = Point.Subtract(EndPoint, start);
				return Point.Add(start, v * relPosition);
			}

			double resX = GetPositionX(ref start, relPosition);
			double resY = GetFunction(ref start)(resX);
			return new Point(resX, resY);
		}

		/// <summary>
		/// Gets a tangent on point on this segment defined by relative position.
		/// This method doesn't checks, if relative position is in range (0, 1).
		/// </summary>
		/// <param name="start">The start point of the segment.</param>
		/// <param name="relPosition">The relative position on the segment.</param>
		/// <returns>A tangent on point on the segment.</returns>
		public override Vector GetTangentOnSegment(ref Point start, double relPosition)
		{
			Refresh(ref start);
			if (a == 0)
			{
				Vector v = Point.Subtract(EndPoint, start);
				v.Normalize();
				return v;
			}

			double resX = GetPositionX(ref start, relPosition);

			double tg = 2 * a * resX + b;
			Vector vv = new Vector(1, tg);
			vv.Normalize();
			return vv;
		}

		/// <summary>
		/// Gets a functional values in defined x position of the ParabolicArcSegment.
		/// </summary>
		/// <param name="start">The start point of the parabolic arc segment.</param>
		/// <param name="x">The x position.</param>
		/// <param name="global">True, if x and returned y value is are global space, false, if x and y are related to the start point.</param>
		/// <param name="tolerance">Tolerance</param>
		/// <returns>A values related to the x position.</returns>
		public override double[] GetValueByX(ref Point start, double x, bool global, double tolerance = 0)
		{
			Refresh(ref start);
			if (!global)
			{
				x += start.X;
			}

			if (Comparators.InIntervalBoth(start.X, EndPoint.X, x, tolerance))
			{
				// y = ax^2 + bx + c
				var y = a * x * x + b * x + c;
				if (!global)
				{
					y -= start.Y;
				}

				return new double[] { y };
			}

			return new double[0];
		}

		/// <summary>
		/// Gets a functional values in defined y position.
		/// </summary>
		/// <param name="start">The start point of the segment.</param>
		/// <param name="y">The y position.</param>
		/// <param name="global">True, if y and returned x value is are global space, false, if y and x are related to the start point.</param>
		/// <param name="tolerance">Tolerance</param>
		/// <returns>A values related to the y position.</returns>
		public override double[] GetValueByY(ref Point start, double y, bool global, double tolerance = 0)
		{
			Refresh(ref start);
			if (!global)
			{
				y += start.Y;
			}

			if (Comparators.InIntervalBoth(start.Y, EndPoint.Y, y, tolerance))
			{
				// y = ax^2 + bx + c
				// ax^2 + bx + (c - y) = 0;
				var cc = c - y;

				var discr = (b * b) - 4 * a * cc;
				if (discr.IsGreater(0, 1e-5) && !a.IsZero())
				{
					var x1 = (-b + Math.Sqrt(discr)) / (2 * a);
					var x2 = (-b - Math.Sqrt(discr)) / (2 * a);

					if (Comparators.InIntervalBoth(start.X, EndPoint.X, x1, tolerance) && Comparators.InIntervalBoth(start.X, EndPoint.X, x2, tolerance))
					{
						return new double[]
						{
							global ? x1 : x1 - start.X,
							global ? x2 : x2 - start.X,
						};
					}
					else if (Comparators.InIntervalBoth(start.X, EndPoint.X, x1, tolerance))
					{
						return new double[]
						{
							global ? x1 : x1 - start.X
						};
					}
					else if (Comparators.InIntervalBoth(start.X, EndPoint.X, x2, tolerance))
					{
						return new double[]
						{
							global ? x2 : x2 - start.X
						};
					}
				}
			}

			return new double[0];
		}

		/// <summary>
		/// Calculates relative position on segment by given x position.
		/// </summary>
		/// <param name="start">The start point of the segment.</param>
		/// <param name="x">The x position.</param>
		/// <param name="global">True, if x and returned y value is are global space, false, if x and y are related to the start point.</param>
		/// <param name="tolerance">Distance tolerance.</param>
		/// <returns>The relative position.</returns>
		public override double[] GetRelativePosition(ref Point start, double x, bool global, double tolerance = 0)
		{
			Refresh(ref start);
			if (!global)
			{
				x += start.X;
			}

			var l1 = EvaluateLengthIntegral(start.X);
			var l2 = EvaluateLengthIntegral(EndPoint.X);
			var l3 = EvaluateLengthIntegral(x);

			var r = Math.Abs(l1 - l3) / Math.Abs(l1 - l2);
			return new double[] { x < Math.Min(start.X, EndPoint.X) ? -r : r };
		}

		/// <summary>
		/// Calculates a curvature in specified x position.
		/// </summary>
		/// <param name="start">The start point.</param>
		/// <param name="x">The x coordinate.</param>
		/// <returns>The curvature.</returns>
		public override double GetCurvature(ref Point start, double x)
		{
			Refresh(ref start);
			double c = 2 * a / Math.Sqrt(Math.Pow(1 + 4 * a * a * x * x + 4 * a * b * x + b * b, 3));
			return Math.Abs(c);
		}

		#endregion Segment2D overrides

		#region ICloneable Members

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public override object Clone()
		{
			return new ParabolicArcSegment2D(this);
		}

		#endregion ICloneable Members

		#region Public methods

		/// <summary>
		/// Resets manualy the parabola constants.
		/// </summary>
		public void Reset()
		{
			this.startCache = null;
		}

		/// <summary>
		/// Calculates the control point of parabola.
		/// </summary>
		/// <param name="start">The start point of the segment.</param>
		/// <returns>The control point of parabola.</returns>
		public Point GetControlPoint(ref Point start)
		{
			Refresh(ref start);
			Point control;
			Point beg, end;
			var rotation = new Matrix();
			if (!this.angle.IsZero())
			{
				rotation.Rotate(this.angle);
				beg = rotation.Transform(start);
				end = rotation.Transform(EndPoint);
			}
			else
			{
				beg = start;
				end = EndPoint;
			}

			// prusecik tecny na zacatku a  konci paraboly
			var uybeg = (2 * a * start.X) + b;
			var uyend = (2 * a * EndPoint.X) + b;
			var t = uybeg - uyend;
			if (t != 0)
			{
				control = new Point()
				{
					X = beg.X + (1 * ((uyend * (beg.X - end.X)) - (1 * (beg.Y - end.Y))) / t),
					Y = beg.Y + (uybeg * ((uyend * (beg.X - end.X)) - (1 * (beg.Y - end.Y))) / t),
				};
				if (!rotation.IsIdentity)
				{
					control = rotation.Transform(control);
				}
			}
			else
			{
				control = this.point;
			}

			return control;
		}

		/// <summary>
		/// Calculates a minimal curvature of the parabola segment.
		/// </summary>
		/// <param name="start">The start point.</param>
		/// <returns>The minimal curvature.</returns>
		public double GetMinimalCurvature(ref Point start)
		{
			Refresh(ref start);
			var xt = -b / (2 * a);
			if (xt <= Math.Min(start.X, EndPoint.X) || xt >= Math.Max(start.X, EndPoint.X))
			{
				if (Math.Abs(xt - start.X) < Math.Abs(xt - EndPoint.X))
				{
					xt = start.X;
				}
				else
				{
					xt = EndPoint.X;
				}
			}

			return GetCurvature(ref start, xt);
		}

		///// <summary>
		///// Calculates a maximal curvature of the parabola segment. NOT USED
		///// </summary>
		///// <param name="start">The start point.</param>
		///// <returns>The maximal curvature.</returns>
		////public double GetMaximalCurvature(ref Point start)
		////{
		////    //Refresh(ref start);
		////    return b;
		////}

		/// <summary>
		/// Gets the function of this parabolic segment.
		/// </summary>
		/// <param name="start">The start point.</param>
		/// <returns>The function of this segment.</returns>
		public Func<double, double> GetFunction(ref Point start)
		{
			Refresh(ref start);
			return (x) => (a * x * x) + (b * x) + c;
		}

		/// <summary>
		/// Calculates angle of parabola.
		/// </summary>
		/// <param name="start">The start point of segment.</param>
		/// <returns>A total angle of segment.</returns>
		public double GetAngle(ref Point start)
		{
			var t0 = GetTangentOnSegment(ref start, 0);
			var t1 = GetTangentOnSegment(ref start, 1);
			var angle = Vector.AngleBetween(t0, t1);
			return angle;
		}

		/// <summary>
		/// The parabola is concave up (holds water) - convex
		/// </summary>
		/// <param name="start">The start point of the parabolic arc segment.</param>
		/// <returns>True if parabola is concave up</returns>
		public bool IsConcaveUp(ref Point start)
		{
			Refresh(ref start);
			return a.IsGreater(0);
		}

		/// <summary>
		/// The parabola is concave down (sheds water)
		/// </summary>
		/// <param name="start">The start point of the parabolic arc segment.</param>
		/// <returns>True is concave down</returns>
		public bool IsConcaveDown(ref Point start)
		{
			Refresh(ref start);
			return a.IsLesser(0);
		}

		#endregion Public methods

		#region Privates methods

		internal bool CoordinatesFromTangent(ref Point start, Vector tangent, out Point p)
		{
			Refresh(ref start);
			p = new Point
			{
				X = (tangent.Y / tangent.X - b) / (2 * a)
			};
			var yy = GetValueByX(ref start, p.X, true);
			if (yy.Length > 0)
			{
				p.Y = yy[0];
				return true;
			}
			else
			{
				return false;
			}
		}

		private void Refresh(ref Point start)
		{
			if (!this.startCache.HasValue || !this.startCache.Value.IsEqualWithTolerance(start))
			{
				CalculateConstants(ref start);
			}
		}

		/// <summary>
		/// Spocita parametry paraboly v lokalnim sour. systemu paraboly.
		/// </summary>
		/// <param name="start">The start point of the segment.</param>
		/// <returns>true, if points are correct.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1407:ArithmeticExpressionsMustDeclarePrecedence", Justification = "Reviewed by HRO.")]
		private bool CalculateConstants(ref Point start)
		{
			this.startCache = start;
			var pts = new Point[]
			{
				start,
				Point,
				EndPoint,
			};
			if (!this.angle.IsZero())
			{
				var rotation = new Matrix();
				rotation.Rotate(this.angle);
				rotation.Transform(pts);
			}

			var mat = new double[3][];
			mat[0] = new double[3];
			mat[1] = new double[3];
			mat[2] = new double[3];
			mat[0][0] = pts[0].X * pts[0].X;
			mat[0][1] = pts[0].X;
			mat[0][2] = 1;
			mat[1][0] = pts[1].X * pts[1].X;
			mat[1][1] = pts[1].X;
			mat[1][2] = 1;
			mat[2][0] = pts[2].X * pts[2].X;
			mat[2][1] = pts[2].X;
			mat[2][2] = 1;

			var det = Determinant(ref mat);
			if (det != 0)
			{
				// y = ax^2 + bx + c
				// y' = 2ax + b
				a =
					(mat[1][1] * mat[2][2] - mat[1][2] * mat[2][1]) / det * pts[0].Y +
					(mat[0][2] * mat[2][1] - mat[0][1] * mat[2][2]) / det * pts[1].Y +
					(mat[0][1] * mat[1][2] - mat[0][2] * mat[1][1]) / det * pts[2].Y;

				b =
					(mat[1][2] * mat[2][0] - mat[1][0] * mat[2][2]) / det * pts[0].Y +
					(mat[0][0] * mat[2][2] - mat[0][2] * mat[2][0]) / det * pts[1].Y +
					(mat[0][2] * mat[1][0] - mat[0][0] * mat[1][2]) / det * pts[2].Y;

				// parameter c je potreba pouze pro vypocet nulove tecny, viz Boundary
				c =
					(mat[1][0] * mat[2][1] - mat[1][1] * mat[2][0]) / det * pts[0].Y +
					(mat[0][1] * mat[2][0] - mat[0][0] * mat[2][1]) / det * pts[1].Y +
					(mat[0][0] * mat[1][1] - mat[0][1] * mat[1][0]) / det * pts[2].Y;

				return true;
			}

			return false;
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1407:ArithmeticExpressionsMustDeclarePrecedence", Justification = "Reviewed by HRO.")]
		private static double Determinant(ref double[][] mat)
		{
			return
				mat[0][0] * (mat[1][1] * mat[2][2] - mat[1][2] * mat[2][1]) - (
				mat[0][1] * (mat[1][0] * mat[2][2] - mat[1][2] * mat[2][0]) +
				mat[0][2] * (mat[1][1] * mat[2][0] - mat[1][0] * mat[2][1]));
		}

		private double GetPositionX(ref Point start, double relPosition)
		{
			double resX;
			if (relPosition == 0)
			{
				resX = start.X;
			}
			else if (relPosition == 1)
			{
				resX = this.EndPoint.X;
			}
			else
			{
				var param = new IterParams() { Length = relPosition * GetLength(ref start), StartX = start.X };
				Iterations.NewtonMethod(
					delegate(double x, IterParams p)
					{
						var l1 = EvaluateLengthIntegral(p.StartX);
						var l2 = EvaluateLengthIntegral(x);
						return Math.Abs(l1 - l2) - p.Length;
					},
					param,
					////EndPoint.X, ... this is for secant method
					start.X + (relPosition * (EndPoint.X - start.X)),
					1e-6,
					out resX);
			}

			return resX;
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1407:ArithmeticExpressionsMustDeclarePrecedence", Justification = "Reviewed by HRO.")]
		private double EvaluateLengthIntegral(double x)
		{
			return (Math.Sqrt(4 * a * a * x * x + 4 * a * b * x + b * b + 1) * (2 * a * x + b) + Tools.Asinh(2 * a * x + b)) / (4 * a);
		}

		#endregion Privates methods

		#region Private classes and structs

		private struct IterParams
		{
			public double Length;

			public double StartX;
		}

		#endregion Private classes and structs
	}
}