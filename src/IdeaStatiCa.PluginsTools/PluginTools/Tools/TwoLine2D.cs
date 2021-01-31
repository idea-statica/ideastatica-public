using System.Windows;

namespace CI.GiCL2D
{
	public class TwoLine2D
	{
		private Point[] myPoint = new Point[4];
		private CrossStatus[] mystPt = { CrossStatus.And, CrossStatus.And, CrossStatus.And, CrossStatus.And };

		public enum CrossStatus : byte
		{
			Infinite = 0,
			And = 1,
			Not = 2,
		}

		// slouzi pro vyhodnoceni koeficientu usecky
		public enum AbscissaStatus : byte
		{
			Low,		// k < 0
			Begin,	// k == 0
			Inside,	// k > 0 && k < 1
			End,		// k == 1
			Hight		// k > 1
		}

		//		public TwoLine2D()
		//		{
		//			for (int i = 0; i < 4; i++) myPoint[i] = new Point();
		//		}

		public static AbscissaStatus AbscissaStatusFromRel(double k, double limit = Funcs2D.Epson)
		{
			if (Funcs2D.IsZero(k, limit)) return AbscissaStatus.Begin;
			if (Funcs2D.IsZero(k - 1, limit)) return AbscissaStatus.End;
			if (k < 0) return AbscissaStatus.Low;
			if (k > 1) return AbscissaStatus.Hight;
			return AbscissaStatus.Inside;
		}

		public static bool IsOutStatus(AbscissaStatus st)
		{
			return st == AbscissaStatus.Low || st == AbscissaStatus.Hight;
		}

		public static bool IsBoundaryStatus(AbscissaStatus st)
		{
			return st == AbscissaStatus.Begin || st == AbscissaStatus.End;
		}

		public static bool IsInStatus(AbscissaStatus st)
		{
			return st == AbscissaStatus.Inside;
		}

		public TwoLine2D(Line2D first, Line2D second)
		{
			myPoint[0] = first.Begin;
			myPoint[1] = first.End;
			myPoint[2] = second.Begin;
			myPoint[3] = second.End;
		}

		public TwoLine2D(Point a1, Point a2, Point b1, Point b2)
		{
			myPoint[0] = a1;
			myPoint[1] = a2;
			myPoint[2] = b1;
			myPoint[3] = b2;
		}

		public Line2D First
		{
			get { Line2D ret = new Line2D(myPoint[0], myPoint[1]); return ret; }
			set { myPoint[0] = value.Begin; myPoint[1] = value.End; }
		}

		public Line2D Second
		{
			get { Line2D ret = new Line2D(myPoint[2], myPoint[3]); return ret; }
			set { myPoint[2] = value.Begin; myPoint[3] = value.End; }
		}

		public CrossStatus[] StatusPoint
		{
			get { return (CrossStatus[])mystPt.Clone(); }
			set
			{
				int i, l = value.Length;

				if (l > 4) l = 4;
				for (i = 0; i < l; i++) mystPt[i] = value[i];
			}
		}

		public Point this[int inx]
		{
			get { return myPoint[inx]; }
			set { myPoint[inx] = value; }
		}

		public static bool CrossRel(Point pta, Vector da, //bod a vektor prvni primky
											Point ptb, Vector db, //bod a vektor druhe primky
											out Point retRel)  // vraci relativni souradnice
		{
			double t;
			retRel = new Point();

			t = (da.X * db.Y) - (da.Y * db.X);
			if (Funcs2D.IsEqual(t, 0.0, Funcs2D.Epson)) return false;
			retRel.X = ((ptb.X * db.Y) - (ptb.Y * db.X) - (pta.X * db.Y) + (pta.Y * db.X)) / t;
			retRel.Y = ((pta.X * da.Y) - (pta.Y * da.X) - (ptb.X * da.Y) + (ptb.Y * da.X)) / ((db.X * da.Y) - (db.Y * da.X));
			return true;
		}

		public static bool CrossRel(Point a1, Point a2, //dva body prvni primky
											Point b1, Point b2, //dva body druhe primky
											out Point retRel) // vraci relativni souradnice
		{
			Vector da = a2.Minus(a1),
							db = b2.Minus(b1);

			return CrossRel(a1, da, b1, db, out retRel);
		}

		private static bool testRelBeginPt(CrossStatus st, double t)
		{
			switch (st)
			{
				case CrossStatus.Infinite: break;
				case CrossStatus.And: if (!(t > 0 || Funcs2D.IsEqual(t, 0.0, Funcs2D.Epson))) return false;
					break;

				case CrossStatus.Not: if (Funcs2D.IsEqual(t, 0.0, Funcs2D.Epson)) return false;
					if (!(t > 0)) return false;
					break;
			}
			return true;
		}

		private static bool testRelEndPt(CrossStatus st, double t)
		{
			switch (st)
			{
				case CrossStatus.Infinite: break;
				case CrossStatus.And: if (!(t < 1 || Funcs2D.IsEqual(t, 1.0, Funcs2D.Epson))) return false;
					break;

				case CrossStatus.Not: if (Funcs2D.IsEqual(t, 1.0, Funcs2D.Epson)) return false;
					if (!(t < 1)) return false;
					break;
			}
			return true;
		}

		public static bool TestRelOnLine(CrossStatus both, double t)
		{
			return TestRelOnLine(both, both, t);
		}

		public static bool TestRelOnLine(CrossStatus begin, CrossStatus end, double t)
		{
			if (!testRelBeginPt(begin, t)) return false;
			if (!testRelEndPt(end, t)) return false;
			return true;
		}

		// vypocita prusecik dvou primek ktere jsou definovane pocatecnim bodem 'pxx' a usekem 'sxx'
		// vraci true pokud nejsou primky rovnobezne nebo prusecik lezi v definovane oblesti podle eStatus
		// priklad :	primka  - (eStatus.Infinite , eStatus.Infinite}
		//						usecka vcetne pocatecniho bodu a bez koncoveho bodu - (eStatus.And , eStatus.Not}
		public static bool CrossAbs(Point p11, CrossStatus s11, Vector p12, CrossStatus s12,
											Point p21, CrossStatus s21, Vector p22, CrossStatus s22,
											out Point retPt)																										// vraci absolutni souradnice pruseciku
		{
			retPt = new Point();
			if (!CrossRel(p11, p12, p21, p22, out Point u)) return false;

			if (!TestRelOnLine(s11, s12, u.X)) return false;
			if (!TestRelOnLine(s21, s22, u.Y)) return false;

			retPt.X = p11.X + (u.X * p12.X);
			retPt.Y = p11.Y + (u.X * p12.Y);
			return true;
		}

		public static bool CrossAbs(Point p11, Vector p12,
												Point p21, Vector p22,
												CrossStatus sall, out Point retPt)									// vraci absolutni souradnice pruseciku
		{
			return CrossAbs(p11, sall, p12, sall, p21, sall, p22, sall, out retPt);
		}

		// vypocita prusecik dvou primek ktere jsou definovane pocatecnim a koncovym bodem
		// vraci true pokud nejsou primky rovnobezne nebo prusecik lezi v definovane oblesti podle eStatus
		public static bool CrossAbs(Point a1, CrossStatus sa1, Point a2, CrossStatus sa2,
											Point b1, CrossStatus sb1, Point b2, CrossStatus sb2,
											out Point retPt)																						// vraci absolutni souradnice pruseciku
		{
			Vector da = a2.Minus(a1),
						 db = b2.Minus(b1);

			return CrossAbs(a1, sa1, da, sa2, b1, sb1, db, sb2, out retPt);
		}

		public static bool CrossAbs(Point a1, Point a2,
												Point b1, Point b2,
												CrossStatus sall, out Point retPt)																// vraci absolutni souradnice pruseciku
		{
			return CrossAbs(a1, sall, a2, sall, b1, sall, b2, sall, out retPt);
		}

		public bool CrossRel(ref Point ret)
		{
			return CrossRel(myPoint[0], myPoint[1], myPoint[2], myPoint[3], out ret);
		}

		public bool CrossAbs(ref Point ret)
		{
			return CrossAbs(myPoint[0], mystPt[0], myPoint[1], mystPt[1], myPoint[2], mystPt[2], myPoint[3], mystPt[3], out ret);
		}

		// zjisti zda jsou usecky korektne zadane, nemaji nulovou delku
		public int IsNotCorrectLine()
		{
			Line2D l = new Line2D(myPoint[0], myPoint[1]);
			if (Funcs2D.IsEqual(l.Length, 0, Funcs2D.Epson)) return 1;
			l.Begin = myPoint[2]; l.End = myPoint[3];
			if (Funcs2D.IsEqual(l.Length, 0, Funcs2D.Epson)) return 2;
			return 0;
		}

		/*
				public bool IsParallel
				{
					get
					{
						if (IsNotCorrectLine() != 0) return false;
						return true;
					}
				}
		 */

		public enum ParallelResult
		{
			/// <summary>
			/// Neni paralelni
			/// </summary>
			Nothing,

			/// <summary>
			/// paralelni nelezi na jedne primce
			/// </summary>
			ParallelOut,

			/// <summary>
			/// paralelni a lezi na jedne primce
			/// </summary>
			ParallelOn
		}

		public static ParallelResult ParallelCheck(
			Point pta, Vector da, //bod a vektor prvni primky
			Point ptb, Vector db, //bod a vektor druhe primky
			out Point retRel,  // vraci relativni souradnice ( x = begin, y = end) na prvni primce
			out bool direct)	// smer vektoru primek, true stejny smer, false opacny smer
		{
			retRel = new Point();
			direct = false;
			var rr = Funcs2D.IsParallel(da, db);
			if(rr == 0) return ParallelResult.Nothing;
			direct = rr > 0;

			retRel.X = Funcs2D.PointToLine(pta, da, ptb);
			retRel.Y = Funcs2D.PointToLine(pta, da, ptb.Plus(db));

			if (Funcs2D.IsThreePointsOnLine(pta, pta.Plus(da), ptb))
				return ParallelResult.ParallelOn;
			else
				return ParallelResult.ParallelOut;
		}

		public static ParallelResult ParallelCheck(
			Point a1, Point a2, //dva body prvni primky
			Point b1, Point b2, //dva body druhe primky
			out Point retRel,  // vraci relativni souradnice ( x = begin, y = end) na prvni primce
			out bool direct)	// smer vektoru primek, true stejny smer, false opacny smer
		{
			Vector da = a2.Minus(a1),
							db = b2.Minus(b1);

			return ParallelCheck(a1, da, b1, db, out retRel, out direct);
		}
	}
}