using System;
using System.Windows;
using System.Collections.Generic;

namespace CI.GiCL2D
{
	/// <summary>
	/// Objeket pro vypocet oblouku
	/// </summary>
#if !SILVERLIGHT

	[System.Reflection.Obfuscation(Feature = "renaming")]
#endif
	public struct Arc2D
	{
		/// <summary>
		/// pocatecni bod
		/// </summary>
		public Point Begin;

		/// <summary>
		/// koncovy bod
		/// </summary>
		public Point End;

		/// <summary>
		/// polomer, + = kratky oblouk , - = dlouhy oblouk
		/// </summary>
		public double Radius;

		/// <summary>
		/// smer
		/// </summary>
		public bool Clockwise;

		/// <summary>
		/// oblouk neni definovany
		/// </summary>
		public bool IsEmpty { get { return Funcs2D.IsEqual(Radius, 0); } }

		/// <summary>
		/// vyvori objekt kruznice
		/// </summary>
		public Circle2D Circle2D { get { return new Circle2D(Begin, End, Clockwise ? Radius : -Radius); } }

		/// <summary>
		/// vypocita maximalni hranice
		/// </summary>
		public Rect2D Boundary
		{
			get
			{
				return GetBoundary();
			}
		}

		/// <summary>
		/// vytvori oblouk zadany dvema body a polomerem
		/// </summary>
		/// <param name="begin">pocatecni bod</param>
		/// <param name="end">koncovy bod</param>
		/// <param name="radius">polomer, + = kratky oblouk , - = dlouhy oblouk</param>
		/// <param name="clockwise">smer</param>
		public Arc2D(Point begin, Point end, double radius, bool clockwise)
		{
			Begin = begin;
			End = end;
			if (Funcs2D.IsEqual(begin, end))
				Radius = 0;
			else
				Radius = radius;
			Clockwise = clockwise;
		}

		/// <summary>
		/// oblouk zadany stredem , pocatecnim bodem a uhlem
		/// </summary>
		/// <param name="center">stred</param>
		/// <param name="pt">pocatecni bod</param>
		/// <param name="angle">uhel v radianech  , smer podle znamenka ( - = clockwise)</param>
		public Arc2D(Point center, Point pt, double angle)
		{
			Radius = Funcs2D.Distance(center, pt);
			Begin = pt;
			if (Math.Abs(angle) > Math.PI * 2) angle = angle % (Math.PI * 2);
			Clockwise = angle < 0;
			double a = Funcs2D.Radian(Vector.AngleBetween(new Vector(1, 0), pt.Minus(center)));
			a += angle;
			End = Circle2D.PointOn(center, Radius, a);
			if (Math.Abs(angle) > Math.PI)
				Radius *= -1;
		}

		/// <summary>
		/// oblouk zadany stredem , pocatecnim bodem a vektorem ne kterem lezi kocovy bod
		/// </summary>
		/// <param name="center">stred</param>
		/// <param name="pt">pocatecni bod</param>
		/// <param name="vector">vektor na kterem lezi koncovy bod</param>
		/// <param name="shortangle">true = kratky oblouk (mezi nulou az 180 stupni), false = dlouhy oblouk (doplnek do 360 stupnu)</param>
		public Arc2D(Point center, Point pt, Vector vector, bool shortangle = true)
		{
			Radius = Funcs2D.Distance(center, pt);
			Begin = pt;
			double a = Funcs2D.Radian(Vector.AngleBetween(new Vector(1, 0), vector));
			End = Circle2D.PointOn(center, Radius, a);
			if (!shortangle) Radius *= -1;
			a = Vector.AngleBetween(pt.Minus(center), vector);
			Clockwise = !(a > 0 && shortangle || a < 0 && !shortangle);
		}

		/// <summary>
		/// oblouk zadany tremi body
		/// </summary>
		/// <param name="pt1">pocatecni bod</param>
		/// <param name="pt2">bod uvnitr oblouku</param>
		/// <param name="pt3">koncovy bod</param>
		public Arc2D(Point pt1, Point pt2, Point pt3)
		{
			var c = new Circle2D(pt1, pt2, pt3);
			Begin = pt1;
			End = pt3;
			Radius = c.Radius;
			if (Funcs2D.IsEqual(Radius, 0)) Clockwise = true;
			else
			{
				Vector b = pt1.Minus(c.Center);
				double a1 = Vector.AngleBetween(b, pt2.Minus(c.Center));
				double a2 = Vector.AngleBetween(b, pt3.Minus(c.Center));

				if (a1 < 0) a1 = 360 + a1;
				if (a2 < 0) a2 = 360 + a2;
				Clockwise = a1 > a2;
				if ((Clockwise && a2 < 180) || (!Clockwise && a2 > 180)) Radius *= -1;
			}
		}

		/// <summary>
		/// Oblouk tvoreny stredem, polomerem a dvema uhly
		/// </summary>
		/// <param name="center">stred</param>
		/// <param name="radius">polomer</param>
		/// <param name="startangle">pocatecni uhel</param>
		/// <param name="angle">uhel sevreny pocatecnim a koncovym bodem oblouku</param>
		public Arc2D(Point center, double radius, double startangle, double angle)
		{
			Radius = Math.Abs(radius);
			if (Math.Abs(angle) > Math.PI * 2) angle = angle % (Math.PI * 2);
			if (Math.Abs(startangle) > Math.PI * 2) startangle = startangle % (Math.PI * 2);
			Begin = Circle2D.PointOn(center, Radius, startangle);
			End = Circle2D.PointOn(center, Radius, startangle + angle);
			Clockwise = angle < 0;
			if (Math.Abs(angle) > Math.PI) Radius *= -1;
		}

		/// <summary>
		/// oblouk tvoreny dvema po sobe jdoucimi useckami a polomerem
		/// takzvane zaobleni rohu
		/// pokud je polomer zaporny vytvori se tzv. vykousnuti  se stredem ve spolecnem bode
		/// objekt se nastavi na empty pokud jsou usecky rovnobezne nebo jedna z nich ma nulovou delku
		/// </summary>
		/// <param name="pt1">pocatecni bod prvni ridici usecky</param>
		/// <param name="pt2">spolecny bod ridicich usecek</param>
		/// <param name="pt3">konecny bod druhe ridici usecky</param>
		/// <param name="radius">polomer zaobleni, zaporny polomer provede vykousnuti</param>
		/// <param name="testPt">pokud se pocatecni nebo koncovy bod vypocita mimo usecku, objekt se nastavi na Empty</param>
		public Arc2D(Point pt1, Point pt2, Point pt3, double radius, bool testPt = false)
		{
			// zjistim smer offsetu
			double a = Vector.AngleBetween(pt2.Minus(pt1), pt3.Minus(pt2));
			if (a > 0)
			{
				a = -Math.Abs(radius);
				Clockwise = false;
			}
			else
			{
				a = Math.Abs(radius);
				Clockwise = true;
			}

			// vypocitam prusecik offsetovanych primek
			Point[] p1 = Funcs2D.LineOffset(pt1, pt2, a);
			Point[] p2 = Funcs2D.LineOffset(pt2, pt3, a);
			if (p1 == null || p2 == null || !TwoLine2D.CrossRel(p1[0], p1[1], p2[0], p2[1], out Point rr))
			{
				// tri body na jedne primce
				Begin = pt1; End = pt3;
				Radius = 0;
				return;
			}
			if (radius > 0)
			{
				if (testPt)
				{
					if (!TwoLine2D.TestRelOnLine(TwoLine2D.CrossStatus.And, rr.X) || !TwoLine2D.TestRelOnLine(TwoLine2D.CrossStatus.And, rr.Y))
					{
						// body jsou mimo usecky
						Begin = pt1; End = pt3;
						Radius = 0;
						return;
					}
				}

				// koeficienty prenesu do puvodnich usecek a spocitam body
				Begin = Funcs2D.PointOnLine(pt1, pt2, rr.X);
				End = Funcs2D.PointOnLine(pt2, pt3, rr.Y);
				Radius = radius;
			}
			else
			{
				Radius = -radius;
				if (testPt)
				{
					if (Funcs2D.Distance(pt1, pt2) < Radius || Funcs2D.Distance(pt2, pt3) < Radius)
					{
						// body jsou mimo usecky
						Begin = pt1; End = pt3;
						Radius = 0;
						return;
					}
				}
				// bod na prvni primce ve vzdalenosti abs(radius) od spolecneho bodu
				Begin = Funcs2D.PointOnLineLen(pt2,pt1,Radius);
				// bod na druhe primce
				End = Funcs2D.PointOnLineLen(pt2, pt3, Radius);
				Clockwise = !Clockwise;
			}
		}

		/// <summary>
		/// vytvori oblouk zadany dvema body a zdvihem v poloine usecky ktera je tvorena body
		/// </summary>
		/// <param name="pt1">pocatecni bod oblouku</param>
		/// <param name="pt2">koncovy bod oblouky</param>
		/// <param name="elevation">zdvih oblouku v polovine usecky [pt1,pt2]</param>
		/// <returns></returns>
		public static Arc2D Arc2DElevation(Point pt1, Point pt2, double elevation)
		{
			return Arc2DElevation(pt1, pt2.Minus(pt1), elevation);
		}

		/// <summary>
		/// vytvori oblouk zadany pocatecnim bodem, vektorem ve kterm je koncovy bod a zdvihem v poloine usecky 
		/// </summary>
		/// <param name="pt1">pocatecni bod oblouku</param>
		/// <param name="v">vektor udavajici koncovy bod oblouky</param>
		/// <param name="elevation">zdvih oblouku v polovine usecky [pt1,v]</param>
		/// <returns></returns>
		public static Arc2D Arc2DElevation(Point pt1, Vector v, double elevation)
		{
			Point pt2, pt3 = pt1.Plus(v);
			pt2 = Funcs2D.PointOnLineLen(Funcs2D.PointOnLine(pt1, v, 0.5), Funcs2D.VectorNormal(v), elevation);
			return new Arc2D(pt1, pt2, pt3);
		}

		/// <summary>
		/// iplicitni operator pretypovani na objekt kruznice
		/// </summary>
		/// <param name="src">zdrojovy objekt oblouku</param>
		/// <returns>novy objekt kruznice</returns>
		public static implicit operator Circle2D(Arc2D src)
		{
			return new Circle2D(src.Begin, src.End, src.Clockwise ? src.Radius : -src.Radius);
		}

		/// <summary>
		/// vypocita uhly definovane obloukem, uhly jsou v radianech
		/// </summary>
		/// <param name="start">pocatecni uhel oblouku</param>
		/// <param name="angle">uhel sevreny pocatecnim a koncovym bodem oblouky, zaporny uhel  - oblouk je ve smeru hodinovych rucicek</param>
		/// <returns>pokud je objekt empty vrati false</returns>
		public Circle2D? GetAngles(out double start, out double angle)
		{
			Circle2D c = Circle2D;
			start = angle = 0;
			if (c.IsEmpty) return null;
			Vector b = Begin.Minus(c.Center);
			double ab = Vector.AngleBetween(new Vector(1, 0), b);
			double a = Vector.AngleBetween(b, End.Minus(c.Center));

			if (ab < 0) ab = 360 + ab;

			if (a < 0) a = 360 + a;
			if (Clockwise) a -= 360;
			start = Funcs2D.Radian(ab);
			angle = Funcs2D.Radian(a);
			return c;
		}

		/// <summary>
		/// vrati tri body na oblouku
		/// </summary>
		/// <param name="k2">relativni delkova souradnice druheho bodu</param>
		/// <returns>vrati pole bodu  ret[0] = zacatek, ret[1] = bod na oblouku podle k2, ret[2] = konec</returns>
		public Point[] ThreePoints(double k2 = 0.5)
		{
			if (IsEmpty) return null;
			var ret = new Point[3];
			ret[0] = Begin;

			Circle2D c = Circle2D;
			Vector b = Begin.Minus(c.Center);
			double ab = Vector.AngleBetween(new Vector(1, 0), b);
			double a = Vector.AngleBetween(b, End.Minus(c.Center));

			if (ab < 0) ab = 360 + ab;

			if (a < 0) a = 360 + a;
			if (Clockwise) a -= 360;
			a = ab + (a * k2);
			ret[1] = c.PointOn(Funcs2D.Radian(a));

			ret[2] = End;
			return ret;
		}

		/// <summary>
		/// otestuje zda poloprimka zadaneho smeru protne oblouk
		/// </summary>
		/// <param name="angle">uhel poloprimky</param>
		/// <returns></returns>
		public bool IsAngleOn(double angle)
		{
			if (GetAngles(out double st, out double an) == null) return false;
			angle = Funcs2D.PureRadianAngle(angle);
			if (an > 0)
			{
				an = st + an;
				return angle.IsGreaterOrEqual(st) && angle.IsLesserOrEqual(an);
			}
			else
			{
				an = st + an;
				return angle.IsGreaterOrEqual(an) && angle.IsLesserOrEqual(st);
			}
		}

		/// <summary>
		/// otestuje zda prislusny bot lezi na oblouku
		/// </summary>
		/// <param name="pt">testovany bod</param>
		/// <returns></returns>
		public bool IsPointOn(Point pt)
		{
			Circle2D? c = GetAngles(out double st, out double an);
			if (c == null) return false;
			if (!Funcs2D.Distance(pt, c.Value.Center).IsEqual(Math.Abs(Radius))) return false;
			st = Funcs2D.PureRadianAngle(st);
			double angle = Funcs2D.LineAngle(c.Value.Center, pt);
			if (an > 0)
			{
				an = st + an;
				return angle.IsGreaterOrEqual(st) && angle.IsLesserOrEqual(an);
			}
			else
			{
				an = st + an;
				return angle.IsGreaterOrEqual(an) && angle.IsLesserOrEqual(st);
			}
		}

		/// <summary>
		/// vypocita meze oblouku
		/// </summary>
		/// <returns>vypocitane meze</returns>
		public Rect GetBoundary()
		{
			Circle2D c = Circle2D;

			double a;
			var ret = new Rect(Begin, End);
			for (int i = 0; i < 4; i++)
			{
				a = i * (Math.PI / 2);
				if (IsAngleOn(a)) ret.Union(c.PointOn(a));
			}
			return ret;
		}

		/// <summary>
		/// vytvori polygon delenim oblouku
		/// </summary>
		/// <param name="koef">predpis pro deleni
		/// keof vetsi nez  0  - uhlovy segment v radianech
		/// koef mensi nebo rovno  0 - absolutni hodnota urcuje pocet dilu na ktere se segment rozdeli
		/// </param>
		/// <param name="withoutBegin">urcuje zda ma vratit pole bez prvniho bodu</param>
		/// <returns>Vrati pole bodu, pokud je oblouk empty vrati null </returns>
		public Point[] Discretization(double koef, bool withoutBegin)
		{
			Circle2D? c = GetAngles(out double start, out double angle);
			Point[] ret = null;
			if (c.HasValue)
			{
				if (koef > 0)
				{
					var l = new List<Point>();
					if (!withoutBegin) l.Add(Begin);
					double d = 0;
					if (angle < 0)
					{
						koef = -koef;
						for (d = koef; d >= angle; d += koef)
						{
							l.Add(c.Value.PointOn(start + d));
						}
					}
					else
					{
						for (d = koef; d <= angle; d += koef)
						{
							l.Add(c.Value.PointOn(start + d));
						}
					}

					if (Funcs2D.IsEqual(angle, d))
					{
						//Bugfix #14454
						if (l.Count != 0)
						{
							l[l.Count - 1] = End;
						}
						else
						{
							l.Add(End);
						}
					}

					ret = l.ToArray();
				}
				else
				{
					int num = (int)Math.Abs(koef);
					if (num != 0)
					{
						int i = 1;
						if (withoutBegin)
						{
							i = 0;
						}
						ret = new Point[num + i];
						if(!withoutBegin) ret[0] = Begin;
						angle /= num;
						for (double d = angle; i < num; i++, d +=angle)
						{
							ret[i] = c.Value.PointOn(start + d);
						}
					}
				}
			}
			return ret;
		}
	}
}