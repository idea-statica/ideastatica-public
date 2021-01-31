using System.Windows;

namespace CI.GiCL2D
{
#if !SILVERLIGHT

	[System.Reflection.Obfuscation(Feature = "renaming")]
#endif
	public class Line2D : IGeomEntity2D
	{
		private Point mybegin;
		private Point myend;

		public Point Begin
		{
			get { return mybegin; }
			set { mybegin = value; }
		}

		public Point End
		{
			get { return myend; }
			set { myend = value; }
		}

		public Line2D()
		{
			mybegin = new Point();
			myend = new Point();
		}

		public Line2D(Line2D src)
		{
			mybegin = src.Begin;
			myend = src.End;
		}

		public Line2D(Point begin, Point end)
		{
			mybegin = begin;
			myend = end;
		}

		public Line2D(double x1, double y1, double x2, double y2)
		{
			mybegin = new Point(x1, y1);
			myend = new Point(x2, y2);
		}

		public double Length
		{
			get { return Funcs2D.Distance(mybegin, myend); }
		}

		public Vector Vector
		{
			get { return myend.Minus(mybegin); }
		}

		public Direction Direction
		{
			get { Direction ret = new Direction(mybegin, myend); return ret; }
		}

		public double this[int inx]
		{
			get
			{
				switch (inx)
				{
					case 0: return mybegin.X;
					case 1: return mybegin.Y;
					case 2: return myend.X;
					case 3: return myend.Y;

					//default : vyjimka
				}
				return 0;
			}
			set
			{
				switch (inx)
				{
					case 0: mybegin.X = value; break;
					case 1: mybegin.Y = value; break;
					case 2: myend.X = value; break;
					case 3: myend.Y = value; break;

					//default : vyjimka
				}
			}
		}

		public void Trans(bool inside, ITransEngine2D te)
		{
			if (inside)
			{
				te.TransTo(mybegin, ref mybegin);
				te.TransTo(myend, ref myend);
			}
			else
			{
				te.TransFrom(mybegin, ref mybegin);
				te.TransFrom(myend, ref myend);
			}
		}
	}
}