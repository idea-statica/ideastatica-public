using System;
using System.Collections.Generic;
using System.Windows;

namespace CI.GiCL2D
{
	public class PolygonReader : IPolygonReader
	{
		#region IPolygonReader Members

		public double[,] CopyTo()
		{
			double[,] ret = new double[Length, 2];
			for (int i = 0; i < Length; i++)
			{
				GetRow(i, out ret[i, 0], out ret[i, 1]);
			}
			return ret;
		}

		public virtual void GetRow(int row, out double x, out double y)
		{
			throw new NotImplementedException("PolygonReader.GetRow");
		}

		public virtual int Length
		{
			get { throw new NotImplementedException("PolygonReader.Length"); }
		}

		#endregion IPolygonReader Members
	};

	public class BoxListPoint : PolygonReader
	{
		private List<Point> myValue;

		public BoxListPoint()
		{
			myValue = new List<Point>();
		}

		public BoxListPoint(List<Point> list)
		{
			Value = list;
		}

		public BoxListPoint(IPolygonReader pr)
		{
			if (pr == null) throw (new ArgumentNullException("pr"));
			myValue = new List<Point>();
			for (int i = 0; i < pr.Length; i++)
			{
				pr.GetRow(i, out double x, out double y);
				myValue.Add(new Point(x, y));
			}
		}

		public List<Point> Value
		{
			get { return myValue; }
			set { if (value == null) myValue = new List<Point>(0); else myValue = value; }
		}

		public override int Length
		{
			get { return myValue.Count; }
		}

		public override void GetRow(int row, out double x, out double y)
		{
			x = myValue[row].X;
			y = myValue[row].Y;
		}
	}

	public class BoxArrayPoint : PolygonReader
	{
		protected Point[] myValue;

		public Point[] Value
		{
			get { return myValue; }
			set { if (value == null) myValue = new Point[0]; else myValue = value; }
		}

		public BoxArrayPoint(int size)
		{
			if (size < 0) size = 0; myValue = new Point[size];
		}

		public BoxArrayPoint(Point[] arr)
		{
			Value = arr;
		}

		public BoxArrayPoint(IPolygonReader pr)
		{
			if (pr == null) throw (new ArgumentNullException("pr"));
			myValue = new Point[pr.Length];
			for (int i = 0; i < pr.Length; i++)
			{
				pr.GetRow(i, out double x, out double y);
				myValue[i].X = x;
				myValue[i].Y = y;
			}
		}

		public override int Length
		{
			get { return myValue.Length; }
		}

		public override void GetRow(int row, out double x, out double y)
		{
			if (row >= Value.Length) throw (new IndexOutOfRangeException());
			x = myValue[row].X;
			y = myValue[row].Y;
		}
	}

	public class BoxTrans : PolygonReader
	{
		private IPolygonReader myPoly;

		public delegate void TransMethod(Point src, ref Point dest);

		private TransMethod d_Trans;

		// zaridi transformaci polygonu pri cteni
		public BoxTrans(IPolygonReader polygon, TransMethod transfuncs)
		{
			if (polygon == null) throw (new ArgumentNullException("BoxTrans(polygon,..)"));
			if (transfuncs == null) throw (new ArgumentNullException("BoxTrans(..,transfuncs)"));
			myPoly = polygon;
			d_Trans = transfuncs;
		}

		public override int Length
		{
			get { return myPoly.Length; }
		}

		public override void GetRow(int row, out double x, out double y)
		{
			if (row >= myPoly.Length) throw (new IndexOutOfRangeException());
			myPoly.GetRow(row, out x, out y);
			Point pt = new Point();
			d_Trans(new Point(x, y), ref pt);
			x = pt.X;
			y = pt.Y;
		}
	}

	public class BoxTransTo : BoxTrans
	{
		public BoxTransTo(IPolygonReader polygon, ITransEngine2D transformation) :
			base(polygon, new TransMethod(transformation.TransTo))
		{ }
	}

	public class BoxTransFrom : BoxTrans
	{
		public BoxTransFrom(IPolygonReader polygon, ITransEngine2D transformation) :
			base(polygon, new TransMethod(transformation.TransFrom))
		{ }
	}
}