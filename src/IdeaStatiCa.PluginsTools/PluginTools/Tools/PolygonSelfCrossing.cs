using System;
using System.Collections.Generic;
using System.Windows;

namespace CI.GiCL2D
{
	public class PolygonSelfCrossing
	{
		private MyLine[] myData;              // data pro vyhodnoceni splitu
		private List<int[]> myCrossId;        // id usecek pruseciku
		private double[,] myPoly;             // vstupni polygon
		private List<List<Point>> mySplits;   // vyseparovane polygony splitem
		private List<List<int>> myInxSplits;				// indexy vyslednych hran polygony
		private DataPTR myPTR;                // ovladac pro vyhodnocovani splitu

		public bool IsClosed { get; private set; }

		public IPolygonReader InputPolygon { get { return new BoxArrayDouble2(myPoly); } }

		public PolygonSelfCrossing(IPolygonReader poly, bool isClosed = true)
		{
			IsClosed = isClosed;
			if (poly == null) throw (new ArgumentNullException("PolygonSelfCrossing(poly)"));
			myCrossId = null;
			mySplits = null;

			myPoly = poly.CopyTo();
			int length = myPoly.GetLength(0);
			myData = new MyLine[length];
			for (int i = 0; i < myData.Length; i++)
			{
				myData[i] = new MyLine(i, i == (myData.Length - 1));
			}
			int begin = 0;
			if (!IsClosed)
			{
				length -= 2;
				begin = 1;
			}
			for (int i = begin; i < length; i++)
			{
				for (int j = i + 2; j < length; j++)
				{
					if (myData[i].TestCrossing(myData[j], myPoly))
					{
						if (myCrossId == null) myCrossId = new List<int[]>();
						myCrossId.Add(new int[] { i, j });
					}
				}
			}
			for (int i = 0; i < myData.Length; i++) myData[i].Sort();
		}

		public bool IsCrossing { get { return myCrossId != null; } }

		// return format - Object[,2] {Point,int[2]};
		// Point = crosspoint
		// int[2] = { index_Line1 , index_Line2 }

		public Object[,] GetCrossPoints()
		{
			if (!IsCrossing) return null;
			Object[,] ret = new Object[myCrossId.Count, 2];
			for (int i = 0; i < myCrossId.Count; i++)
			{
				ret[i, 1] = new int[] { myCrossId[i][0], myCrossId[i][1] };
				ret[i, 0] = myData[myCrossId[i][0]].CrossPoint(myCrossId[i][1], myPoly);
			}
			return ret;
		}

		private bool f_MakeSplits()
		{
			if (myCrossId == null) return false;
			if (mySplits == null)
			{
				mySplits = new List<List<Point>>();
				myInxSplits = new List<List<int>>();
				myPTR.Reset();
				f_GetOnePolygonSplit();
			}
			return true;
		}

		// rozdeli polygon v bodech krizeni na jedntlive dilci polygony
		// pokud neni selfCrossing vraci null
		public IPolygonReader[] GetSplitPolygons()
		{
			if (!f_MakeSplits()) return null;
			IPolygonReader[] ret = new IPolygonReader[mySplits.Count];
			for (int i = 0; i < ret.Length; i++) ret[i] = new BoxListPoint(mySplits[i]);
			return ret;
		}

		//vrati navic index hran Split polygonu
		public Tuple<IPolygonReader, List<int>>[] GetSplitPolygonsAndInx()
		{
			if (!f_MakeSplits()) return null;
			var ret = new Tuple<IPolygonReader, List<int>>[mySplits.Count];
			for (int i = 0; i < ret.GetLength(0); i++)
			{
				ret[i] = new Tuple<IPolygonReader, List<int>>(new BoxListPoint(mySplits[i]), myInxSplits[i]);
			}

			return ret;
		}

		//rekurze pro jeden polygon
		private void f_GetOnePolygonSplit()
		{
			List<Point> rpt = new List<Point>();
			List<int> rptInx = new List<int>();
			int start = -1;
			if (myPTR.inxLn != -1)
			{
				if (!Funcs2D.IsEqual(myPTR.kRel, 1, Funcs2D.Epson))// zapisu bod pokud neni prusecik na konci usecky
				{
					rpt.Add(myPTR.pPt);
					rptInx.Add(myPTR.inxLn);
				}
				start = myPTR.inxLn;
			}

			while (f_NextPTR())  // najdu dalsi bod
			{
				if (myPTR.IsCrossing && myPTR.inxCrLn == start) break; // uzavrela se moje smycka
				if (myPTR.IsCrossing) f_GetOnePolygonSplit(); // jdu dovnitr dalsi smycky
				if (!Funcs2D.IsEqual(myPTR.kRel, 1, Funcs2D.Epson))// zapisu bod pokud neni prusecik na konci usecky
				{
					rpt.Add(myPTR.pPt); // zapisu bod
					rptInx.Add(myPTR.inxLn);
				}
			}
			mySplits.Add(rpt);
			myInxSplits.Add(rptInx);
		}

		private bool f_NextPTR()
		{
			if (myPTR.inxLn == -1)
			{
				//musim inicializovat
				myPTR.inxLn = 0;
			}

			if (myPTR.inxLn >= myData.Length) return false;

			if (!myData[myPTR.inxLn].NextPoint(ref myPTR, myPoly)) // pokusim se najit dalsi bod na usecce
			{
				// pokud neni dalsi bod musim prejit na dalsi usecku
				myPTR.inxLn++;
				if (myPTR.inxLn >= myData.Length) return false;
				myData[myPTR.inxLn].NextPoint(ref myPTR, myPoly);
			}
			return true;
		}

		private class MyCross : IComparable
		{
			public double Rel { get; set; }

			public int Inx { get; set; }

			public MyCross(double rel, int inx)
			{
				Rel = rel; Inx = inx;
			}

			int IComparable.CompareTo(object obj)
			{
				MyCross o = (MyCross)obj;
				if (Rel > o.Rel) return 1;
				if (Rel < o.Rel) return -1;
				return 0;
			}

			public override string ToString()
			{
				return string.Format("({1:G};{0})", Inx, Rel);
			}
		}

		private class MyLine
		{
			private int myBegin,
				 myEnd;

			private List<MyCross> myCross;

			public MyLine(int begin, bool isEnd)
			{
				myBegin = begin;
				myEnd = isEnd ? 0 : begin + 1;
				myCross = null;
			}

			public bool TestCrossing(MyLine src, double[,] p)
			{
				if (myEnd == src.myBegin) return false;
				if (myBegin == src.myEnd) return false;
				if (!TwoLine2D.CrossRel(new Point(p[myBegin, 0], p[myBegin, 1]), new Point(p[myEnd, 0], p[myEnd, 1]),
											new Point(p[src.myBegin, 0], p[src.myBegin, 1]), new Point(p[src.myEnd, 0], p[src.myEnd, 1]), out Point t)) return false;
				if (!TwoLine2D.TestRelOnLine(TwoLine2D.CrossStatus.And, t.X)) return false;
				if (!TwoLine2D.TestRelOnLine(TwoLine2D.CrossStatus.And, t.Y)) return false;
				if (Funcs2D.IsEqual(t.X, 0, Funcs2D.Epson) || Funcs2D.IsEqual(t.Y, 0, Funcs2D.Epson)) return false; // prusecik je na zacatku usecky, nezapisu
				f_AddCross(t.X, src.myBegin);
				src.f_AddCross(t.Y, myBegin);
				return true;
			}

			private void f_AddCross(double rel, int inx)
			{
				//        if (GiCL2D.IsEqualDouble(rel, 0, GiCL2D.Epson)) return;
				if (myCross == null) myCross = new List<MyCross>();
				myCross.Add(new MyCross(rel, inx));
			}

			public bool IsCrossing { get { return myCross != null; } }

			public Point CrossPoint(int inx, double[,] p)
			{
				int i = -1;
				for (i = 0; i < myCross.Count; i++) if (myCross[i].Inx == inx) break;
				return Funcs2D.PointOnLine(new Point(p[myBegin, 0], p[myBegin, 1]), new Point(p[myEnd, 0], p[myEnd, 1]), myCross[i].Rel);
			}

			public Point BeginPoint(double[,] p)
			{
				return new Point(p[myBegin, 0], p[myBegin, 1]);
			}

			public void Sort()
			{
				if (myCross != null) myCross.Sort();
			}

			public bool NextPoint(ref DataPTR ptr, double[,] p)
			{
				//Debug.Assert(ptr.inxPt == myBegin);
				if (ptr.inxPt == -1)// pocatecni bod
				{
					//DODO POZOR muze byt prusecik na zacatku,
					ptr.inxCrLn = -1;
					ptr.inxPt = 0;
					ptr.pPt = BeginPoint(p);
					ptr.kRel = 0;
					return true;
				}
				if (IsCrossing && ptr.inxPt < myCross.Count)
				{
					ptr.pPt = Funcs2D.PointOnLine(new Point(p[myBegin, 0], p[myBegin, 1]), new Point(p[myEnd, 0], p[myEnd, 1]), myCross[ptr.inxPt].Rel);
					ptr.kRel = myCross[ptr.inxPt].Rel;
					ptr.inxCrLn = myCross[ptr.inxPt].Inx;
					ptr.inxPt++;
					return true;
				}
				ptr.inxCrLn = -1;
				ptr.inxPt = -1;
				return false;
			}
		}

		private struct DataPTR
		{
			public int inxLn;      // index usecky
			public int inxPt;   // index bodu na usecce  , 0 pocatecni bod, >1 prusecik, -1 = reset
			public Point pPt;       // nalezeny bod
			public int inxCrLn;    // index protinajici usecky
			public double kRel;    // posledni relativni souradnice

			public bool IsCrossing { get { return inxPt > 0; } }

			public void Reset()
			{
				inxLn = -1;
				inxPt = -1;
				inxCrLn = -1;
				kRel = 0;
			}
		}
	}
}