using System;
using System.Collections.Generic;
using System.Windows;

namespace CI.GiCL2D
{
	public class PolygonOffset //: BasePolygonReader
	{
		private MyPoint[] mySrc;
		private List<List<Point>> myRet;
		private List<List<int>> myRetInx;

		public bool IsOutline { get; private set; }

		public bool IsClosed { get; private set; }

		private bool myIsReversed;

		#region public funkce

		public PolygonOffset(IPolygonReader poly, bool isClosed = true, bool isOutline = true)
		{
			// testovani a vyhozeni vyjimek
			if (poly == null) throw (new ArgumentNullException("PolygonOffset(poly,..)"));
			if (poly.Length < (isClosed ? 3 : 2)) throw (new FormatException("PolygonOffset, small number of points"));

			mySrc = new MyPoint[poly.Length];
			myRet = null;

			IsClosed = isClosed;
			IsOutline = isOutline;

			// zjistim smer otaceni
			myIsReversed = Funcs2D.PolygonIsClockwise(poly) == IsOutline;
			for (int i = 0; i < poly.Length; i++)
			{
				poly.GetRow(i, out double x, out double y);
				mySrc[i].Pt.X = x;
				mySrc[i].Pt.Y = y;
				mySrc[i].Id = i;
				mySrc[i].Offset = 0;
			}

			if (!IsClosed) mySrc[poly.Length - 1].Id = 0;
		}

		public IPolygonReader[] MakeOffset(double offset)
		{
			for (int i = 0; i < mySrc.Length; i++)
			{
				mySrc[i].Offset = offset;
			}
			f_MakeOffset();
			return f_MakeResults();
		}

		private IPolygonReader[] f_MakeResults()
		{
			IPolygonReader[] ret = new IPolygonReader[myRet.Count];
			for (int i = 0; i < ret.Length; i++)
			{
				ret[i] = new BoxListPoint(myRet[i]);
			}
			return ret;
		}

		public IPolygonReader[] MakeOffset(params double[] offsets)
		{
			if (offsets == null) throw (new ArgumentNullException("PolygonOffset.MakeOffset(offsets)"));
			if (offsets.Length < mySrc.Length)
				if (!IsClosed && offsets.Length < mySrc.Length - 1)
					throw new FormatException(); // dodam mensi pocet bodu offsetu nez je bodu polygonu

			for (int i = 0; i < mySrc.Length; i++)
			{
				mySrc[i].Offset = offsets[mySrc[i].Id];
			}
			f_MakeOffset();
			return f_MakeResults();
		}

		// vrati informace o tom , z jakeho useku vznikla hrana vysledneho polygonu
		public List<int>[] Indexes
		{
			get
			{
				if (myRetInx == null) return null;
				var ret = new List<int>[myRetInx.Count];
				for (int i = 0; i < ret.Length; i++)
				{
					ret[i] = myRetInx[i];
				}
				return ret;
			}
		}

		#endregion public funkce

		#region lokalni funkce

		private void f_MakeOffset()
		{
			// vytvorim pruseciky
			Object[] robj = f_MakeCross();
			List<Point> cross = (List<Point>)robj[0];
			var retInx = (List<int>)robj[1];
			myRet = new List<List<Point>>();
			myRetInx = new List<List<int>>();

			// pokud doslo ke krizeni vysepruji jednotlive smycky na polygony
			PolygonSelfCrossing pcr = new PolygonSelfCrossing(new BoxListPoint(cross), IsClosed);
			if (pcr.IsCrossing)

			//    if(false)
			{
				// vyseparuji polygony
				var split = pcr.GetSplitPolygonsAndInx();

				// pro vsechny vypocitam plochu
				List<SplitPoly> areas = new List<SplitPoly>(split.Length);
				for (int i = 0; i < split.GetLength(0); i++)
				{
					areas.Add(new SplitPoly(split[i].Item1, split[i].Item2));
				}

				// rozhodnu ktery polygon je hlavni
				// vnejsi smycky maji obraceny smer. , vyhazu je
				bool clkw;
				for (int i = areas.Count - 1; i >= 0; i--)
				{
					clkw = areas[i].IsClkw;
					if (myIsReversed) clkw = !clkw;
					if (clkw == IsOutline) areas.RemoveAt(i);
				}

				//Vnitrni smycky lezi uvnitr hlavni
				areas.Sort();

				// pocitam ze hlavni smycka ma nejvetsi plochu
				/*        BoxListPoint box;
								if (areas.Count > 0)
								{
									box = new BoxListPoint(areas[0].Poly);// areas bylo setrideno podle plochy, vezmu prvni
									areas[0].UpdateInx(myRetInx);
									myRetInx = areas[0].Inx;
								}
								else
									box = new BoxListPoint(cross); // pokud jsem vsechny vyhazel, jako zachranu vezmu cross
						  myRet.Add(box.Value);

						  // hlavni polygon zapisu do vystupnich dat
				 */

				// zapisu vsechny
				foreach (var pl in areas)
				{
					myRet.Add(new BoxListPoint(pl.Poly).Value);
					pl.UpdateInx(retInx);
					myRetInx.Add(pl.Inx);
				}
			}
			else
			{
				myRet.Add(cross);
				myRetInx.Add(retInx);
			}
		}

		private Object[] f_MakeCross()
		{
			Object[] ret = new Object[2];
			List<Point> retPt = new List<Point>();
			List<int> retInx = new List<int>();
			Point[] bln = null, eln; // ofsetovane usecky
			int b, m, e, len;
			if (IsClosed)
			{
				b = mySrc.Length - 1;
				m = 0;
				len = mySrc.Length;
			}
			else
			{
				b = 0;
				m = 1;
				len = mySrc.Length - 1;
			}

			for (; m < len; m++)
			{
				e = m + 1;
				if (e == mySrc.Length) e = 0;

				// test totoznych bodu
				if (Funcs2D.IsEqual(mySrc[b].Pt, mySrc[m].Pt, Funcs2D.Epson))
				{
					b = m;
					continue;  // vynecham
				}

				// test tri body na jedne primce
				if (Funcs2D.IsThreePointsOnLine(mySrc[b].Pt, mySrc[m].Pt, mySrc[e].Pt, Funcs2D.Epson))
				{
					b = m;
					continue;  // vynecham
				}

				//vypocitam offsety
				if (bln == null) bln = Funcs2D.LineOffset(mySrc[b].Pt, mySrc[m].Pt, mySrc[b].Offset);
				eln = Funcs2D.LineOffset(mySrc[m].Pt, mySrc[e].Pt, mySrc[m].Offset);

				if (!IsClosed && b == 0)
				{
					retPt.Add(bln[0]);
					retInx.Add(0);
				}

				// prusecik
				TwoLine2D.CrossAbs(bln[0], bln[1],
											eln[0], eln[1],
											TwoLine2D.CrossStatus.Infinite, out Point rpt);
				retPt.Add(rpt);
				retInx.Add(m);

				if (!IsClosed && e == len)
				{
					retPt.Add(eln[1]);
					retInx.Add(len);
				}

				b = m;
				bln = eln;
			}
			if (len == 1)
			{
				bln = Funcs2D.LineOffset(mySrc[b].Pt, mySrc[m].Pt, mySrc[b].Offset);
				retPt.Add(bln[0]);
				retInx.Add(0);
				retPt.Add(bln[1]);
				retInx.Add(1);
			}
			ret[0] = retPt;
			ret[1] = retInx;
			return ret;
		}

		#endregion lokalni funkce

		/*   #region BasePolygonReader
    public override int Length	// vrati pocet vrcholu polygonu
    {
      get
      {
        if (myRet==null) return 0;
        return myRet.Count;
      }
    }

    public override void GetRow(int row, out double x, out double y)
    {
      if (row >= Length) throw (new IndexOutOfRangeException());
      x = myRet[row].X;
      y = myRet[row].Y;
    }

  #endregion BasePolygonReader

*/

		private struct MyPoint
		{
			public Point Pt;
			public int Id;
			public double Offset;
		}

		private class SplitPoly : IComparable
		{
			public IPolygonReader Poly;
			public List<int> Inx;
			private double Area;
			public bool IsClkw;

			public SplitPoly(IPolygonReader poly, List<int> inx)
			{
				Poly = poly;
				Inx = inx;
				Area = Funcs2D.PolygonArea(poly);
				IsClkw = Area <= 0;
				Area = Math.Abs(Area);
			}

			int IComparable.CompareTo(object obj)
			{
				SplitPoly o = (SplitPoly)obj;
				if (Area > o.Area) return -1;
				if (Area < o.Area) return 1;
				return 0;
			}

			// prepisi indexy podle mapy vstupnich bodu
			public void UpdateInx(List<int> m)
			{
				for (int i = 0; i < Inx.Count; i++)
				{
					if (Inx[i] >= m.Count) continue;
					Inx[i] = m[Inx[i]];
				}
			}
		}
	}
}