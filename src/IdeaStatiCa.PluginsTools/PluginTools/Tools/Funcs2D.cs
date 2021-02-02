using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;

namespace CI.GiCL2D
{
	/// <summary>
	/// Zakladni geometricke funkce
	/// </summary>
	public static partial class Funcs2D
	{
		public const double Epson = 1e-10;

		/// <summary>
		/// More precise definition of PI
		/// </summary>
		public const double PI = 3.14159265358979323846;

		// testuje rovnost dvou cisel , s povolenou odchylkou 'limit'
		public static bool IsEqual(double v1, double v2, double limit)
		{
			return Math.Abs(v2 - v1) <= limit;
		}

		public static bool IsZero(double p, double limit)
		{
			return IsEqual(p, 0, limit);
		}

		public static bool IsEqual(double v1, double v2) // testuje rovnost dvou cisel , s povolenou odchylkou 'Epson'
		{
			return IsEqual(v1, v2, Epson);
		}

		public static bool IsZero(double p)
		{
			return IsEqual(p, 0);
		}

		public static int Compare(double v1, double v2, double limit)// tridici funkce
		{
			if (IsEqual(v1, v2, limit)) return 0;
			if (v1 > v2) return 1;
			return -1;
		}

		public static int Compare(double v1, double v2)// tridici funkce pro double collection
		{
			return Compare(v1, v2, Epson);
		}

		public static bool IsEqual(Point pt1, Point pt2, double limit) // testuje rovnost dvou bodu , s povolenou odchylkou 'limit'
		{
			return IsEqual(pt1.X, pt2.X, limit) && Funcs2D.IsEqual(pt1.Y, pt2.Y, limit);
		}

		public static bool IsEqual(Point pt1, Point pt2) // testuje rovnost dvou bodu , s povolenou odchylkou 'Epson'
		{
			return IsEqual(pt1, pt2, Epson);
		}

		public static bool IsEqual(Vector v1, Vector v2, double limit) // testuje rovnost dvou vektoru , s povolenou odchylkou 'limit'
		{
			return IsEqual(v1.X, v2.X, limit) && Funcs2D.IsEqual(v1.Y, v2.Y, limit);
		}

		public static bool IsEqual(Vector v1, Vector v2) // testuje rovnost dvou vektoru , s povolenou odchylkou 'Epson'
		{
			return IsEqual(v1, v2, Epson);
		}

		public static bool IsNull(Vector v, double limit) // testuje nulovy vektor , s povolenou odchylkou 'limit'
		{
			return IsEqual(v.X, 0, limit) && Funcs2D.IsEqual(v.Y, 0, limit);
		}

		public static bool IsNull(Vector v) // testuje nulovy vektor , s povolenou odchylkou 'Epson'
		{
			return IsNull(v, Epson);
		}

		// podminka tri body na jedne primce
		public static bool IsThreePointsOnLine(Point b1, Point b2, Point b3, double limit)
		{
			double a, b;
			a = (b2.X - b1.X) * (b3.Y - b1.Y);
			b = (b2.Y - b1.Y) * (b3.X - b1.X);
			if (!IsEqual(a, b, limit)) return false;
			return true;
		}

		public static bool IsThreePointsOnLine(Point b1, Point b2, Point b3)
		{
			return IsThreePointsOnLine(b1, b2, b3, Epson);
		}

		// testuje rovnobeznost dvou vektoru
		public static int IsParallel(Vector v1, Vector v2, double limit)
		{
			// vyloucim nulove vektory
			if (IsEqual(v1.X, 0, limit) && IsEqual(v1.Y, 0, limit)) return 0;
			if (IsEqual(v2.X, 0, limit) && IsEqual(v2.Y, 0, limit)) return 0;

			double k1 = (v1.X * v2.Y) - (v1.Y * v2.X);
			if (IsEqual(k1, 0.0, limit * limit))
			{
				if (Vector.AngleBetween(v1, v2).IsZero()) return 1;
				else return -1;
			}
			return 0;
		}

		public static int IsParallel(Vector v1, Vector v2)
		{
			return IsParallel(v1, v2, Epson);
		}

		// testuje rovnobeznost dvou primek
		public static int IsParallel(Point a1, Point a2, Point b1, Point b2, double limit)
		{
			return IsParallel(a2.Minus(a1), b2.Minus(b1), limit);
		}

		public static int IsParallel(Point a1, Point a2, Point b1, Point b2)
		{
			return IsParallel(a1, a2, b1, b2, Epson);
		}

		// testuje kolmost dvou vektoru
		public static bool IsNormall(Vector v1, Vector v2, double limit)
		{
			return IsParallel(v1, VectorNormal(v2), limit) != 0;
		}

		public static bool IsNormall(Vector v1, Vector v2)
		{
			return IsNormall(v1, v2, Epson);
		}

		// testuje kolmost dvou primek
		public static bool IsNormall(Point a1, Point a2, Point b1, Point b2, double limit)
		{
			return IsNormall(a2.Minus(a1), b2.Minus(b1), limit);
		}

		public static bool IsNormall(Point a1, Point a2, Point b1, Point b2)
		{
			return IsNormall(a1, a2, b1, b2);
		}

		// vrati normalu vektoru
		public static Vector VectorNormal(Vector src)
		{
			return new Vector(src.Y, -src.X);
		}

		// vrati jednotkovy vektor v danem smeru
		public static Vector VectorAngle(double angle)
		{
			return new Vector(Math.Cos(angle), Math.Sin(angle));
		}

		// vypocte bod na primce
		public static Point PointOnLine(Point pt, Vector v, double rel) // vrati absolutni bod na usecce [pt,v], ktery je urceny relativni sourdnici 'rel'
		{
			Point retPt = new Point
			{
				X = pt.X + (rel * v.X),
				Y = pt.Y + (rel * v.Y)
			};
			return retPt;
		}

		public static Point PointOnLine(Point a1, Point a2, double rel) // vrati absolutni bod na usecce [a1,a2], ktery je urceny relativni sourdnici 'rel'
		{
			return PointOnLine(a1, a2.Minus(a1), rel);
		}

		// 
		/// <summary>
		/// Vypocte bod na primce ve vzdalenosti 'len' od pocatku
		/// </summary>
		/// <param name="pt">pocatecni bod primky</param>
		/// <param name="v">Vektor primky</param>
		/// <param name="len">vzdalenost od pocatku</param>
		/// <returns>vrati absolutni bod na usecce [pt,v]</returns>
		public static Point PointOnLineLen(Point pt, Vector v, double len)
		{
			var rel = len / v.Length;
			return PointOnLine(pt, v, rel);
		}

		// 
		/// <summary>
		/// Vypocte bod na primce ve vzdalenosti 'len' od pocatku
		/// </summary>
		/// <param name="pt">pocatecni bod primky</param>
		/// <param name="v">konecny bod primky</param>
		/// <param name="len">vzdalenost od pocatku</param>
		/// <returns>vrati absolutni bod na usecce [a1,a2]</returns>
		public static Point PointOnLineLen(Point a1, Point a2, double len)
		{
			return PointOnLineLen(a1, a2.Minus(a1), len);
		}

		//vypocita kolmy prumet bodu na usecce , vysledek je relativni souradnice usecky
		public static double PointToLine(Point pt1, Vector v, Point pk)
		{
			Vector vn = Funcs2D.VectorNormal(v);

			TwoLine2D.CrossRel(pt1, v, pk, vn, out Point ret);

			return ret.X;
		}

		//vypocita kolmy prumet bodu na usecce , vysledek je absolutni bod
		public static Point PointToLine(Point a1, Point a2, Point pk)
		{
			return PointOnLine(a1, a2, PointToLine(a1, a2.Minus(a1), pk));
		}

		// vypocita rovnobeznou primku
		public static Point[] LineOffset(Point pt1, Point pt2, double offset)
		{
			Vector vv = pt2.Minus(pt1), // vektor
			nv = Funcs2D.VectorNormal(vv);            // kolmy vektor

			Point[] ret = new Point[2];
			if (Funcs2D.IsNull(vv))
			{
				ret[0] = pt1;
				ret[1] = pt2;
				return ret;
			};

			double len = vv.Length,               // delka primky
			kn = offset / len;              // delkovy koeficient offsetu

			// prvni bod
			ret[0].X = pt1.X + (kn * nv.X);
			ret[0].Y = pt1.Y + (kn * nv.Y);

			// druhy bod
			ret[1].X = ret[0].X + vv.X;
			ret[1].Y = ret[0].Y + vv.Y;

			return ret;
		}

		//vypocita bod lezici na kolmici spustene v relativnim  bodu primky 'rel' ve vzdalenosti 'offset'
		public static Point LineRelPtOffset(Point pt1, Point pt2, double rel, double offset)
		{
			return LineRelPtOffset(pt1, pt2.Minus(pt1), rel, offset);
		}

		public static Point LineRelPtOffset(Point pt1, Vector vv, double rel, double offset)
		{
			Point ret = new Point();
			Vector nv = Funcs2D.VectorNormal(vv);            // kolmy vektor
			if (Funcs2D.IsNull(vv)) return ret;

			double len = vv.Length,               // delka primky
							kn = offset / len;              // delkovy koeficient offsetu
			Point pt = PointOnLine(pt1, vv, rel);

			ret.X = pt.X + (kn * nv.X);
			ret.Y = pt.Y + (kn * nv.Y);

			return ret;
		}

		// vypocita vzdalenost dvou bodu
		public static double Distance(Point pt1, Point pt2)
		{
			return Math.Sqrt(Math.Pow(pt2.X - pt1.X, 2) + Math.Pow(pt2.Y - pt1.Y, 2));
		}

		public static double Degrees(double radian)
		{
			return radian * 180 / PI;
		}

		public static double Radian(double degrees)
		{
			return degrees * PI / 180;
		}

		//ze dvou bodu vypocita jednotkovy vektor
		public static Direction UnitVector(Point origin, Point pt)
		{
			Direction ret = new Direction(origin, pt);
			return ret;
		}

		// tridici funkce pro Point
		public static int CompareX(Point p1, Point p2)
		{
			return Compare(p1.X, p2.X);
		}

		public static int CompareY(Point p1, Point p2)
		{
			return Compare(p1.Y, p2.Y);
		}

		/*		public static int CompareX(Vector p1, Vector p2)
				{
					return p1.X.CompareTo(p2.X);
				}
		*/

		public class ComparerDouble : IComparer<double>
		{
			private double myEpson;

			public ComparerDouble()
			{
				myEpson = Funcs2D.Epson;
			}

			public ComparerDouble(double limit)
			{
				myEpson = limit;
			}

			#region IComparer<double> Members

			int IComparer<double>.Compare(double x, double y)
			{
				return Funcs2D.Compare(x, y, myEpson);
			}

			#endregion IComparer<double> Members
		}

		/// <summary>
		/// Prepocita hodnotu uhlu tak aby byl v rozsahu 0 az 360 stupnu
		/// </summary>
		/// <param name="angle"></param>
		/// <returns></returns>
		public static double PureDegreesAngle(double angle)
		{
			double ret = Math.Abs(angle);
			bool minus = (angle < 0);
			ret %= 360;
			if (minus && ret != 0) ret = 360 - ret;
			return ret;
		}

		public static double PureRadianAngle(double angle)
		{
			double ret = Math.Abs(angle), k = PI * 2;
			bool minus = (angle < 0);
			ret %= k;
			if (ret.IsEqual(k)) ret = 0;
			if (minus && ret != 0) ret = k - ret;
			return ret;
		}

		/// <summary>
		/// urci bod dotyku kruznice s tecnou primkou prochazejici bodem
		/// </summary>
		/// <param name="center">stred kruznice</param>
		/// <param name="radius">polomer kruznice, znamenko urcuje vyslednou polorovinu ve ktere se bude nachazet vypocitany bod</param>
		/// <param name="linepoint">bod tecne primky, ( pokud je uvnitr kruznice, vyslednym bodem bude stred kruznice )</param>
		/// <returns>vysledny prusecik tecne primky a kruznice</returns>
		public static Point CircleTangentPoint(Point center, double radius, Point linepoint)
		{
			if (radius.IsZero(1e-6)) return center;
			double l = Distance(center, linepoint);
			if (l.IsLesserOrEqual(Math.Abs(radius), 1e-6)) return center;

			double alfa = Math.Asin(-radius / l);
			double angle = Radian(Vector.AngleBetween(new Vector(1, 0), linepoint.Minus(center)));
			Vector v = VectorAngle(angle + alfa);
			return PointOnLine(linepoint, v, PointToLine(linepoint, v, center));
		}

		public static Point[] TwoCircleTangentPoints(Point center1, double radius1, Point center2, double radius2)
		{
			if (radius1.IsZero()) return new Point[] { center1, CircleTangentPoint(center2, radius2, center1) };
			if (radius2.IsZero()) return new Point[] { CircleTangentPoint(center1, -radius1, center2), center2 };

			bool zn1 = radius1 > 0;
			radius1 = Math.Abs(radius1);
			bool zn2 = radius2 > 0;
			radius2 = Math.Abs(radius2);
			double l = Distance(center1, center2);
			if (zn1 == zn2) // shodne znamenko
			{
				if (l.IsZero(1e-6)) return null;
				if (radius1 <= radius2)
				{
					double rv = radius2 - radius1;
					if (!zn1) rv = -rv;
					Point p = CircleTangentPoint(center2, rv, center1);
					return LineOffset(center1, p, zn1 ? radius1 : -radius1);
				}
				else
				{
					double rv = radius1 - radius2;
					if (zn1) rv = -rv;
					Point p = CircleTangentPoint(center1, rv, center2);
					return LineOffset(p, center2, zn1 ? radius2 : -radius2);
				}
			}
			else
			{
				if (l.IsLesserOrEqual(radius1 + radius2, 1e-6)) return null;
				double k = radius1 / (radius1 + radius2);
				Point p = PointOnLine(center1, center2, k);
				Point[] ret = new Point[2];
				ret[0] = CircleTangentPoint(center1, zn2 ? radius1 : -radius1, p);
				ret[1] = CircleTangentPoint(center2, zn2 ? radius2 : -radius2, p);
				return ret;
			}
		}

		public static bool IsClockwise(Point pt1, Point pt2, Point pt3)
		{
			return Vector.AngleBetween(pt2.Minus(pt1), pt3.Minus(pt2)).IsLesserOrEqual(0);
		}

		//Zkontroluje prubeh zaobleni
		// return 	0 = OK
		//					1 bit = je mimo prvni usecku
		//					2 bit = je mimo druhou usecku
		// 					3 bit = body jsou na jedne primce
		public static int TestRounding(Point pt1, Point pt2, Point pt3, double r)
		{
			int ret = 0;
			double a = Vector.AngleBetween(pt2.Minus(pt1), pt3.Minus(pt2));
			if (a > 0) r = -r;

			// vypocitam prusecik offsetovanych primek
			Point[] p1 = Funcs2D.LineOffset(pt1, pt2, a);
			if (p1 == null) ret |= 1;
			Point[] p2 = Funcs2D.LineOffset(pt2, pt3, a);
			if (p2 == null) ret |= 2;
			if (ret != 0) return ret;
			if (!TwoLine2D.CrossRel(p1[0], p1[1], p2[0], p2[1], out Point rr)) return 4;
			if (!TwoLine2D.TestRelOnLine(TwoLine2D.CrossStatus.And, rr.X)) ret |= 1;
			if (!TwoLine2D.TestRelOnLine(TwoLine2D.CrossStatus.And, rr.Y)) ret |= 2;
			return ret;
		}

		public static double LineAngle(Point pt1, Point pt2)
		{
			return VectorAngle(pt2.Minus(pt1));
		}

		public static double VectorAngle(Vector v)
		{
			return PureRadianAngle(Radian(Vector.AngleBetween(new Vector(1, 0), v)));
		}

		public static double RoundingScale(double src, out int actDigit)
		{
			actDigit = 0;
//#if !SILVERLIGHT
			if (src.IsZero())
			{
				//Debug.Fail("Funcs2D.RoundingScale()   src == 0");
				return 0;
			}
//#endif
			bool isNegative = src < 0;
			bool isUp;
			int nn = 0;
			src = Math.Abs(src);
			double ret = src;
			if (src < 1) // budu nasobit
			{
				isUp = true;
				while (ret < 1)
				{
					ret *= 10;
					nn++;
				}
			}

			//budu delit
			else
			{
				isUp = false;
				while (ret > 10)
				{
					ret /= 10;
					nn++;
				}
			}
			ret = ScaleRange(ret);
			for (int i = 0; i < nn; i++)
			{
				if (isUp) ret /= 10;
				else ret *= 10;
			}
			if (isNegative) ret *= -1;
			if (isUp) actDigit = -nn;
			else actDigit = nn;
			actDigit--;

			return ret;
		}

		public static double ScaleRange(double src)
		{
			if (src < 1.5) return 1;
			if (src < 2.25) return 2;
			if (src < 3.75) return 2.5;
			if (src < 6.75) return 5;
			if (src < 8) return 7.5;
			return 10;
		}

		/// <summary>
		/// returns rectangle dimensions, rectangle is created from area and outline
		/// </summary>
		/// <param name="origY">original region max dimension Y</param>
		/// <param name="origZ">original region max dimension Z</param>
		/// <param name="area">area of region</param>
		/// <param name="outline">outline of region</param>
		/// <param name="height">alternative rectangle height</param>
		/// <param name="width">alternative rectangle width</param>
		public static void GetRectangleFromAreaAndOutline(double origY, double origZ, double area, double outline, out double height, out double width)
		{
			double discriminant = outline * outline - 16 * area;
			if (discriminant.IsLesser(0))
			{
				height = origZ;
				width = origY;
			}
			else
			{
				double bk1 = (-outline - Math.Pow(discriminant, 0.5)) / (-4);
				double bk2 = (-outline + Math.Pow(discriminant, 0.5)) / (-4);
				double hk1 = (outline - 2 * bk1) / 2;
				double hk2 = (outline - 2 * bk2) / 2;

				if (hk1.IsGreater(0) && bk1.IsGreater(0) && hk2.IsGreater(0) && bk2.IsGreater(0))
				{
					if (origZ.IsEqual(origY) && origZ.IsGreater(origY))
					{
						origZ = origY;
					}

					if (origZ.IsLesser(origY))
					{
						height = Math.Min(hk1, hk2);
						width = Math.Max(bk1, bk2);
					}
					else
					{
						height = Math.Max(hk1, hk2);
						width = Math.Min(bk1, bk2);
					}
				}
				else
				{
					height = origZ;
					width = origY;
				}
			}
		}

		/// <summary>
		/// Check quad if is convex
		/// </summary>
		/// <param name="x0">area edge point coordinate</param>
		/// <param name="y0">area edge point coordinate</param>
		/// <param name="x1">area edge point coordinate</param>
		/// <param name="y1">area edge point coordinate</param>
		/// <param name="x2">area edge point coordinate</param>
		/// <param name="y2">area edge point coordinate</param>
		/// <param name="x3">area edge point coordinate</param>
		/// <param name="y3">area edge point coordinate</param>
		/// <param name="outPoint">Index of point outside of convex area 0-3</param>
		/// <returns>true if area is convex</returns>
		public static bool IsAreaConvex(double x0, double y0, double x1, double y1, double x2, double y2, double x3, double y3, out int outPoint)
		{
			double min = 5.0;
			Point p0 = new Point(x0, y0);
			Point p1 = new Point(x1, y1);
			Point p2 = new Point(x2, y2);
			Point p3 = new Point(x3, y3);

			Vector v0 = p1.Minus(p0);
			Vector v1 = p2.Minus(p1);
			Vector v2 = p3.Minus(p2);
			Vector v3 = p0.Minus(p3);

			if (Vector.AngleBetween(v0, v1) <= min)
			{
				outPoint = 1;
				return false;
			}

			if (Vector.AngleBetween(v1, v2) <= min)
			{
				outPoint = 2;
				return false;
			}

			if (Vector.AngleBetween(v2, v3) <= min)
			{
				outPoint = 3;
				return false;
			}

			if (Vector.AngleBetween(v3, v0) <= min)
			{
				outPoint = 0;
				return false;
			}

			outPoint = 0;
			return true;
		}
	}

	/// <summary>
	/// The extension methods.
	/// </summary>
	public static class Extension
	{
		#region Double extension methods

		/// <summary>
		/// Checks, if value is zero with specified tolerance.
		/// </summary>
		/// <param name="value">The value for check.</param>
		/// <param name="tolerance">The precision of check.</param>
		/// <returns>True, if value is zero, false otherwise.</returns>
		public static bool IsZero(this double value, double tolerance = 1e-9)
		{
			return Math.Abs(value) < tolerance;
		}

		/// <summary>
		/// Correction of double value acc. to limit
		/// </summary>
		/// <param name="value">value</param>
		/// <param name="limit">limit</param>
		/// <returns>double</returns>
		public static double Correct(this double value, double limit = 1e0)
		{
			if (limit.IsZero(1e-12))
			{
				return value;
			}

			double d = value / limit;
			//// dd = d > 0 ? Math.Floor(d) : Math.Ceiling(d);
			double dd = Math.Floor(d);
			if (d - dd >= 0.5)
			{
				dd += 1;
			}

			return dd * limit;
		}

		/// <summary>
		/// IsEqual - Determines whether leftValue and rightValue are equal.
		/// </summary>
		/// <param name="leftValue">Value on LHS of operator</param>
		/// <param name="rightValue">Value on RHS of operator</param>
		/// <param name="tolerance">Tolerance level for comparison</param>
		/// <returns>Return true if leftValue and rightValue are equal. Return false otherwise</returns>
		public static bool IsEqual(this double leftValue, double rightValue, double tolerance = 1e-10)
		{
			return Math.Abs(leftValue - rightValue) <= tolerance;
		}

		/// <summary>
		/// IsGreater - Determines whether the leftValue is greater than rightValue
		/// </summary>
		/// <param name="leftValue">Value on LHS of operator</param>
		/// <param name="rightValue">Value on RHS of operator</param>
		/// <param name="tolerance">Tolerance level for comparison</param>
		/// <returns>Return true if the given leftValue is greater than rightValue. Return false otherwise</returns>
		public static bool IsGreater(this double leftValue, double rightValue, double tolerance = 1e-10)
		{
			return (leftValue - rightValue) >= tolerance;
		}

		/// <summary>
		/// IsGreaterOrEqual - Determines whether the leftValue is greater or equal to rightValue
		/// </summary>
		/// <param name="leftValue">Value on LHS of operator</param>
		/// <param name="rightValue">Value on RHS of operator</param>
		/// <param name="tolerance">Tolerance level for comparison</param>
		/// <returns>Return true if leftValue is greater than or equal to rightValue. Return false otherwise</returns>
		public static bool IsGreaterOrEqual(this double leftValue, double rightValue, double tolerance = 1e-10)
		{
			return (rightValue - leftValue) <= tolerance;
		}

		/// <summary>
		/// IsLesser - Determines whether leftValue is lesser than rightValue
		/// </summary>
		/// <param name="leftValue">Value on LHS of operator</param>
		/// <param name="rightValue">Value on RHS of operator</param>
		/// <param name="tolerance">Tolerance level for comparison</param>
		/// <returns>Return true if the given leftValue is lesser than rightValue. Return false otherwise</returns>
		public static bool IsLesser(this double leftValue, double rightValue, double tolerance = 1e-10)
		{
			return (rightValue - leftValue) >= tolerance;
		}

		/// <summary>
		/// IsLesserOrEqual - Determines whether the leftValue is lesser or equal to rightValue
		/// </summary>
		/// <param name="leftValue">Value on LHS of operator</param>
		/// <param name="rightValue">Value on RHS of operator</param>
		/// <param name="tolerance">Tolerance level for comparison</param>
		/// <returns>Return true if the given leftValue is lesser than or equal to rightValue. Return false otherwise</returns>
		public static bool IsLesserOrEqual(this double leftValue, double rightValue, double tolerance = 1e-10)
		{
			return (leftValue - rightValue) <= tolerance;
		}

		/// <summary>
		/// IsEqual - Determines whether leftValue and rightValue are equal.
		/// </summary>
		/// <param name="leftValue">Value on LHS of operator</param>
		/// <param name="rightValue">Value on RHS of operator</param>
		/// <param name="tolerance">Tolerance level for comparison</param>
		/// <returns>Return true if leftValue and rightValue are equal. Return false otherwise</returns>
		public static bool IsEqual(this Point leftValue, Point rightValue, double tolerance = 1e-10)
		{
			return leftValue.X.IsEqual(rightValue.X, tolerance) && leftValue.Y.IsEqual(rightValue.Y, tolerance);
		}

		#endregion Double extension methods
	}

	public static class Point_extension
	{
		public static Vector Minus(this Point point1, Point point2)
		{
			return new Vector(point1.X - point2.X, point1.Y - point2.Y);
		}

		public static Point Plus(this Point point, Vector delta)
		{
			return new Point(point.X + delta.X, point.Y + delta.Y);
		}
	}
}