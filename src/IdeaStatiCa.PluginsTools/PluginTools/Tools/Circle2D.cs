using System;
using System.Collections.Generic;
using System.Windows;

namespace CI.GiCL2D
{
	/// <summary>
	/// objekt kruznice ve 2D
	/// </summary>
#if !SILVERLIGHT

	[System.Reflection.Obfuscation(Feature = "renaming")]
#endif
	public struct Circle2D
	{
		/// <summary>
		/// stred
		/// </summary>
		public Point Center;

		/// <summary>
		/// polomer
		/// </summary>
		public double Radius;

		/// <summary>
		/// plocha
		/// </summary>
		public double Area
		{
			get { return Math.PI * Radius * Radius; }
		}

		/// <summary>
		/// obvod
		/// </summary>
		public double Circumference
		{
			get { return 2 * Math.PI * Radius; }
		}

		/// <summary>
		/// urcuje zda je spravne definovana
		/// </summary>
		public bool IsEmpty
		{
			get { return Funcs2D.IsEqual(Radius, 0); }
		}

		/// <summary>
		/// vypocita maximalni hranice
		/// </summary>
		public Rect2D Boundary
		{
			get
			{
				if (Radius > 0)
				{
					var ret = new Rect2D(Center.X, Center.Y, 0, 0);
					ret.Inflate(Radius, Radius);
					return ret;
				}
				else
					return new Rect2D(Center.X, Center.Y, 0, 0);
			}
		}

		/// <summary>
		/// kruznice tvorena stredem a polomerem
		/// </summary>
		/// <param name="center">stred</param>
		/// <param name="radius">polomer</param>
		public Circle2D(Point center, double radius)
		{
			Center = center;
			Radius = radius;
		}

		/// <summary>
		/// kruznice zadana stredem a bodem na obvodu
		/// </summary>
		/// <param name="center">stred</param>
		/// <param name="pt">bod na obvodu</param>
		public Circle2D(Point center, Point pt)
		{
			Radius = Funcs2D.Distance(center, pt);
			Center = center;
		}

		/// <summary>
		/// kruznice zadana dvema body na obvodu a polomerem
		/// , pokud je polomer mensi nez polovina vzdalenosti bodu, umisti se stred kruznice do stredoveho bodu tvoriciho obema ridicimi body
		/// , stred kruznice se umisti do poloroviny urcene ridicimi body, podle znamenka polomeru
		/// </summary>
		/// <param name="pt1">prvni bod na obvodu</param>
		/// <param name="pt2">druhy bod na ovodu</param>
		/// <param name="r">polomer</param>
		public Circle2D(Point pt1, Point pt2, double r)
		{
			double d = Funcs2D.Distance(pt1, pt2) / 2;
			if (Math.Abs(r) <= d)
			{
				Radius = d;
				Center = new Point((pt1.X + pt2.X) / 2, (pt1.Y + pt2.Y) / 2);
			}
			else
			{
				Radius = Math.Abs(r);
				double v = Math.Sqrt(Math.Pow(Radius, 2) - Math.Pow(d, 2));
				Center = Funcs2D.LineRelPtOffset(pt1, pt2, 0.5, r < 0 ? -v : v);
			}
		}

		/// <summary>
		/// kruznice urcena tremi body na obvodu
		/// </summary>
		/// <param name="pt1">prvni bod</param>
		/// <param name="pt2">druhy bod</param>
		/// <param name="pt3">treti bod</param>
		public Circle2D(Point pt1, Point pt2, Point pt3)
		{
			Radius = 0;
			Center = pt1;

			Point[] p = new Point[2];
			Vector[] v = new Vector[2];

			p[0].X = (pt1.X + pt2.X) / 2;
			p[0].Y = (pt1.Y + pt2.Y) / 2;
			p[1].X = (pt2.X + pt3.X) / 2;
			p[1].Y = (pt2.Y + pt3.Y) / 2;

			v[0].X = pt2.Y - pt1.Y;
			v[0].Y = -(pt2.X - pt1.X);

			v[1].X = pt3.Y - pt2.Y;
			v[1].Y = -(pt3.X - pt2.X);

			TwoLine2D.CrossStatus st = TwoLine2D.CrossStatus.Infinite;
			if (TwoLine2D.CrossAbs(p[0], st, v[0], st, p[1], st, v[1], st, out Center))
				Radius = Funcs2D.Distance(pt1, Center);
		}

		/// <summary>
		/// vypocita bod na kruznici
		/// </summary>
		/// <param name="center">stred kruznice</param>
		/// <param name="r">polomer kruznice</param>
		/// <param name="angle">uhel v radianech</param>
		/// <returns>vypocitany bod</returns>
		public static Point PointOn(Point center, double r, double angle)
		{
			return new Point(center.X + (r * Math.Cos(angle)), center.Y + (r * Math.Sin(angle)));
		}

		/// <summary>
		/// vypocita bod na kruznici
		/// </summary>
		/// <param name="angle">uhel v radianech</param>
		/// <returns>vypocitany bod</returns>
		public Point PointOn(double angle)
		{
			return new Point(Center.X + Radius * Math.Cos(angle), Center.Y + Radius * Math.Sin(angle));
		}

		public static Point[] PointsCrossLine(Point centre, double r, Point pt1, Point pt2)
		{
			double[] angle = AnglesCrossLine(centre, r, pt1, pt2);
			if (angle == null) return null;
			var ret = new Point[angle.Length];
			for (int i = 0; i < angle.Length; i++)
			{
				ret[i] = PointOn(centre, r, angle[i]);
			}
			return ret;
		}

		public static double[] AnglesCrossLine(Point centre, double r, Point pt1, Point pt2)
		{
			Point t = Funcs2D.PointToLine(pt1, pt2, centre);
			if (Funcs2D.IsEqual(t, centre))
			{
				double an = Funcs2D.LineAngle(pt1, pt2);
				return new double[] { Funcs2D.PureRadianAngle(an + Math.PI), an };
			}
			double z = Funcs2D.Distance(centre, t);
			double a = Funcs2D.LineAngle(centre, t);
			if (z.IsGreater(r)) return null; // mimo
			if (z.IsEqual(r)) // tecna
			{
				return new double[] { a };
			}

			//secna
			double aa = Math.Acos(z / r); // uhel pulky tetivy
			return new double[] { Funcs2D.PureRadianAngle(a + aa), Funcs2D.PureRadianAngle(a - aa) };
		}

		public double[] AnglesCrossLine(Point pt1, Point pt2)
		{
			return AnglesCrossLine(Center, Radius, pt1, pt2);
		}

		public Point[] PointsCrossLine(Point pt1, Point pt2)
		{
			return PointsCrossLine(Center, Radius, pt1, pt2);
		}

		/// <summary>
		/// Indicates, whether Point p lies on the circle (satisfies the equation of circle).
		/// </summary>
		/// <param name="p">The point to check.</param>
		/// <param name="tolerance">The tolerance level for comparison.</param>
		/// <returns>true, if point lies on the circle; false, otherwise.</returns>
		public bool IsPointOn(Point p, double tolerance)
		{
			// (x - m)^2 + (y - n)^2 = r^2
			var lhs = (p.X - this.Center.X) * (p.X - this.Center.X) + (p.Y - this.Center.Y) * (p.Y - this.Center.Y);
			var rhs = this.Radius * this.Radius;
			return lhs.IsEqual(rhs, tolerance);
		}

		/// <summary>
		/// vytvori polygon delenim kruznice
		/// </summary>
		/// <param name="koef">predpis pro deleni
		/// - absolutni hodnota urcuje pocet dilu na ktere se kruznice rozdeli
		/// </param>
		/// <returns>Vrati pole bodu, pokud je kruznice empty vrati null </returns>
		public Point[] Discretization(double koef)
		{
			if (IsEmpty) return null;
			int num = (int)Math.Abs(koef);
			if (num == 0) return null;
			var angle = (Math.PI * 2) / num;
			int i = 0;
			var ret = new Point[num];
			for (double d = 0; i < num; i++, d += angle)
				{
					ret[i] = PointOn(d);
				}
			return ret;
		}
	}
}