using System;
using System.Collections.Generic;
using System.Windows;

namespace CI.GiCL2D
{
	public static partial class Funcs2D
	{
		// polygons

		private static double f_PolygonArea(IPolygonReader poly)
		{
			if (poly == null) throw (new ArgumentNullException("f_PolygonArea(poly)"));
			if (poly.Length < 3) return 0;
			double dx, dy, a = 0;
			double[,] p = poly.CopyTo();
			int b = p.GetLength(0) - 1;
			for (int e = 0; e < p.GetLength(0); e++)
			{
				dx = p[e, 0] - p[b, 0];
				dy = p[e, 1] - p[b, 1];
				a += (p[b, 0] * dy - p[b, 1] * dx) / 2.0;
				b = e;
			}
			return a;
		}

		public static double PolygonArea(IPolygonReader poly)
		{
			return f_PolygonArea(poly);
		}

		public static Point PolygonCentre(IPolygonReader poly)
		{
			if (poly == null) throw (new ArgumentNullException("PolygonCentre(poly)"));
			Point ret = new Point();
			double[] v = new double[3] { 0, 0, 0 };
			double[,] p = poly.CopyTo();
			double dx, dy;
			int b = p.GetLength(0) - 1;
			for (int e = 0; e < p.GetLength(0); e++)
			{
				dx = p[e, 0] - p[b, 0];
				dy = p[e, 1] - p[b, 1];

				v[2] += (p[b, 0] * dy - p[b, 1] * dx) / 2.0;
				v[0] += -dx * (3.0 * p[b, 1] * p[b, 1] + 3.0 * p[b, 1] * dy + dy * dy) / 6.0;
				v[1] += dy * (3.0 * p[b, 0] * p[b, 0] + 3.0 * p[b, 0] * dx + dx * dx) / 6.0;

				b = e;
			}
			ret.X = v[1] / v[2];
			ret.Y = v[0] / v[2];
			return ret;
		}

		public static bool PolygonIsClockwise(IPolygonReader poly)
		{
			return f_PolygonArea(poly) <= 0.0;
		}

		public static Rect2D PolygonBoundary(IPolygonReader poly, ITransEngine2D mat)
		{
			if (poly == null) throw (new ArgumentNullException("PolygonBoundary(poly)"));
			Rect2D ret = Rect2D.Empty;
			Point pt = new Point();
			for (int i = 0; i < poly.Length; i++)
			{
				poly.GetRow(i, out double x, out double y);
				pt.X = x;
				pt.Y = y;
				if (mat != null) mat.TransTo(pt, ref pt);
				if (ret.IsEmpty)
				{
					ret = new Rect2D(pt, new Size(0, 0));
				}
				else
				{
					ret.Union(pt);
				}
			}
			return ret;
		}

		public static Rect2D PolygonBoundary(IPolygonReader[] apoly, ITransEngine2D mat)
		{
			if (apoly == null) throw (new ArgumentNullException("PolygonsBoundary(apoly)"));
			Rect2D ret = Rect.Empty;
			Rect2D p;
			for (int i = 0; i < apoly.Length; i++)
			{
				p = PolygonBoundary(apoly[i], mat);
				if (ret.IsEmpty) ret = p;
				else
				{
					ret.Union(p);
				}
			}
			return ret;
		}

		// vrati relativni souradnice pruseciku primky s polygonem
		public static List<double> PolygonLineCrossingRel(IPolygonReader poly, bool closed, Point pt, Vector v)
		{
			if (poly == null) throw (new ArgumentNullException("PolygonLineCrossing_rel(poly,..)"));
			List<double> ret = new List<double>();
			int b, e;
			double[,] p = poly.CopyTo();
			if (closed)
			{
				e = poly.Length - 1;
				b = 0;
			}
			else
			{
				e = 0;
				b = 1;
			}
			Point p1, p2;
			for (int i = b; i < poly.Length; i++)
			{
				b = e; e = i;
				p1 = new Point(p[b, 0], p[b, 1]);
				p2 = new Point(p[e, 0], p[e, 1]);
				if (TwoLine2D.CrossRel(pt, v, p1, p2.Minus(p1), out Point pr))
				{
					if (TwoLine2D.TestRelOnLine(TwoLine2D.CrossStatus.And, pr.Y))
					{
						ret.Add(pr.X);
					}
				}
			}
			ret.Sort();
			return ret;
		}

		// vrati absolutni souradnice pruseciku primky s polygonem
		public static List<Point> PolygonLineCrossing(IPolygonReader poly, bool closed, Point pt, Vector v)
		{
			List<double> r = PolygonLineCrossingRel(poly, closed, pt, v);
			List<Point> ret = new List<Point>();
			foreach (double rel in r)
			{
				ret.Add(PointOnLine(pt, v, rel));
			}
			return ret;
		}

		// vrati polygon ktery je vycisten od
		// po sobe jdoucich shodnych bodu
		// a bodu ktere lezi na jende primce
		public static List<Point> PolygonPure(IPolygonReader poly, bool closed, double limit)
		{
			if (poly == null) throw (new ArgumentNullException("PolygonPure(poly,..)"));
			Point[] pt = new BoxArrayPoint(poly).Value;
			List<int> retInx = new List<int>(poly.Length);
			f_PolygonPure(pt, closed, limit, ref retInx);
			List<Point> ret = new List<Point>(retInx.Count);
			foreach (int i in retInx)
			{
				ret.Add(pt[i]);
			}
			return ret;
		}

		public static List<Point> PolygonPure(IPolygonReader poly, bool closed)
		{
			return PolygonPure(poly, closed, Epson);
		}

		public static List<int> PolygonPureIndexes(IPolygonReader poly, bool closed, double limit)
		{
			if (poly == null) throw (new ArgumentNullException("PolygonPureIndexes(poly,..)"));
			List<int> retInx = new List<int>(poly.Length);
			f_PolygonPure(new BoxArrayPoint(poly).Value, closed, limit, ref retInx);
			return retInx;
		}

		public static bool PolygonIsPure(IPolygonReader poly, bool closed, double limit)
		{
			if (poly == null) throw (new ArgumentNullException("f_PolygonPure(poly,..)"));
			List<int> ret = null;
			return f_PolygonPure(new BoxArrayPoint(poly).Value, closed, limit, ref ret);
		}

		public static bool PolygonIsPure(IPolygonReader poly, bool closed)
		{
			return PolygonIsPure(poly, closed, Epson);
		}

		private static bool f_PolygonPure(Point[] pt, bool closed, double limit, ref List<int> ret)
		{
			if (pt.Length < 2)
			{
				if (ret != null) ret = new List<int>() { 0 };
				return false;
			}

			int b, m, e, len;
			if (closed)
			{
				// budu zmensovat delku tak dlouho dokud nebude prvni, posledni a predposledni bod na jedne primce
				for (len = pt.Length; len > 2; len--)
					if (!Funcs2D.IsThreePointsOnLine(pt[0], pt[len - 1], pt[len - 2], limit))
						break;
					else
					{
						if (ret == null) return false;// testovani
					}

				// jeste otestuji s poslednim a druhym
				if (!Funcs2D.IsThreePointsOnLine(pt[len - 1], pt[0], pt[1], limit))
				{
					if (ret != null) ret.Add(0);
				}
				else
					if (ret == null) return false;// testovani
			}
			else
			{
				// budu zmensovat delku tak dlouho dokud nebude prvni a posledni bod stejny
				// musim zajistit aby byl polygon skutecne otevreny
				for (len = pt.Length; len > 1; len--)
					if (!Funcs2D.IsEqual(pt[0], pt[len - 1], limit))
						break;
					else
					{
						if (ret == null) return false;// testovani
					}
				if (ret != null) ret.Add(0);
			}

			// skoncim u predposledniho bodu
			len--;

			// zapisu prvni bod

			bool saved = true;

			for (b = 0, m = 1, e = 2; m < len; e++)
			{
				if (Funcs2D.IsThreePointsOnLine(pt[b], pt[m], pt[e], limit))
				{
					if (ret == null) return false;// testovani

					if (m > b + 1)
					{
						if (!Funcs2D.IsThreePointsOnLine(pt[b], pt[b + 1], pt[e], limit))
						{
							if (ret != null) ret.Add(b + 1);
							saved = false;
							b = b + 1;
							m = b + 1;
							e = m;
							continue;
						}
					}

					saved = false;
					m = e;
					continue;  // vynecham
				}
				if (ret != null) ret.Add(m);
				saved = true;
				b = m;
				m = e;
			}

			// zapisu posledni
			if (saved)
			{
				if (ret != null) ret.Add(len); // rovnou zapisu pokud predposledni bod prosel kontrolou
			}
			else
			{
				//zapisu pouze pokud posledni a predposledni neni shodny
				if (!Funcs2D.IsEqual(pt[len - 1], pt[len], limit))
					if (ret != null) ret.Add(len);
			}

			return true;
		}

		public static List<int> PolygonParallelEdges(IPolygonReader poly, bool closed, Vector v)
		{
			List<int> r = new List<int>();
			Point[] p = new BoxArrayPoint(poly).Value;
			for (int i = 0, j; i < p.Length; i++)
			{
				if (i == p.Length - 1) j = 0;
				else j = i + 1;
				if (Funcs2D.IsParallel(v, p[j].Minus(p[i])) != 0) r.Add(i);
			}
			return r;
		}

		public static Point[,] PolygonParallelEdges(IPolygonReader poly, bool closed, Point pt1, Point pt2)
		{
			List<int> r = PolygonParallelEdges(poly, closed, pt2.Minus(pt1));
			Point[,] ret = new Point[r.Count, 2];
			int i = 0, k;
			foreach (int j in r)
			{
				poly.GetRow(j, out double x, out double y);
				ret[i, 0].X = x; ret[i, 0].Y = y;
				if (j == poly.Length - 1) k = 0;
				else k = j + 1;
				poly.GetRow(k, out x, out y);
				ret[i, 1].X = x; ret[i, 1].Y = y;
				i++;
			}
			return ret;
		}

		public static IPolygonReader[] PolygonsInterconnect(IPolygonReader[] polygons, Point pt, Vector v)
		{
			IPolygonReader[] ret = null;
			List<Tuple<double, int, int>> cr = new List<Tuple<double, int, int>>();
			bool[] bcr = new bool[polygons.Length];
			Point ptt = new Point();
			Vector norm = Funcs2D.VectorNormal(v);
			for (int i = 0; i < polygons.Length; i++)
				for (int j = 0; j < polygons[i].Length; j++)
				{
					polygons[i].GetRow(j, out double x, out double y);
					ptt.X = x; ptt.Y = y;
					if (TwoLine2D.CrossRel(pt, v, ptt, norm, out Point pr))
					{
						if (Funcs2D.IsEqual(pr.Y, 0.0, Funcs2D.Epson))
						{
							cr.Add(new Tuple<double, int, int>(pr.X, i, j));
							bcr[i] = true;
						}
					}
				}
			if (cr.Count < 2) return null;
			int k = 1;
			foreach (var b in bcr) if (!b) k++;
			ret = new IPolygonReader[k];
			k = 0;
			for (int i = 0; i < bcr.Length; i++)
			{
				if (!bcr[i]) ret[k++] = polygons[i];
			}

			// trideni
			cr.Sort((a, b) => a.Item1.CompareTo(b.Item1));

			List<Point> rpt = new List<Point>();

			f_InterconnectJoin(0, cr, polygons, rpt);

			ret[k] = new BoxListPoint(Funcs2D.PolygonPure(new BoxListPoint(rpt), true));
			return ret;
		}

		// rekurzivni metoda pro napojeni
		private static void f_InterconnectJoin(int inx, List<Tuple<double, int, int>> cr, IPolygonReader[] polygons, List<Point> rpt)
		{
			int start = cr[inx].Item3; // startovni bod polygonu
			int j = cr[inx].Item2; // index polygonu

			List<Point> p = new BoxListPoint(polygons[j]).Value;
			Tuple<double, int, int> c = null;

			// najdu bod kde se meni polygony
			int k = inx;
			for (; k < cr.Count; k++)
			{
				if (j != cr[k].Item2)
				{
					c = cr[k];
					break;
				}
			}

			if (c != null)
			{
				// vlozim od i po c[k-1].Item1
				// rekurze
				// vlozim zbytek
				for (int i = start; ; i++)
				{
					rpt.Add(p[i]);
					if (i == cr[k - 1].Item3)
					{
						f_InterconnectJoin(k, cr, polygons, rpt);
						rpt.Add(p[i]);
					}
					if (i + 1 == start)
					{
						rpt.Add(p[i + 1]);
						break;
					}
					if (i + 1 == p.Count) i = -1;
				}
			}
			else
			{
				// vlozim cely polygon
				rpt.AddRange(p.GetRange(start, p.Count - start));
				rpt.AddRange(p.GetRange(0, start + 1));
			}
		}
	}
}