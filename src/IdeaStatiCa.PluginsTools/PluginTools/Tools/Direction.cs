using System;
using System.Windows;

namespace CI.GiCL2D
{
#if !SILVERLIGHT

	[System.Reflection.Obfuscation(Feature = "renaming")]
#endif
	public struct Direction : IGeomEntity2D
	{
		private Vector myVc;

		public Direction(double angle)
		{
			myVc = new Vector(Math.Cos(angle), Math.Sin(angle));
		}

		public Direction(double x, double y)
		{
			myVc = new Vector(x, y);
			if (!IsNull()) myVc.Normalize();
		}

		public Direction(Direction d)
		{
			myVc = new Vector(d.X, d.Y);
		}

		public Direction(Point pt1, Point pt2)
		{
			myVc = pt2.Minus(pt1);
			if (!IsNull()) myVc.Normalize();
		}

		public Direction(Vector d)
		{
			myVc = d;
			if (!IsNull()) myVc.Normalize();
		}

		public static Direction Normal(Direction src)
		{
			return new Direction(src.Y, -src.Y);
		}

		public static implicit operator Vector(Direction src)
		{
			return src.myVc;
		}

		public static explicit operator Direction(Vector src)
		{
			return new Direction(src);
		}

		public double X { get { return myVc.X; } }

		public double Y { get { return myVc.Y; } }

		internal double this[int inx]
		{
			get
			{
				switch (inx)
				{
					case 0: return myVc.X;
					case 1: return myVc.Y;
					default: throw new IndexOutOfRangeException();
				}
			}
			set
			{
				switch (inx)
				{
					case 0: myVc.X = value; break;
					case 1: myVc.Y = value; break;
					default: throw new IndexOutOfRangeException();
				}
			}
		}

		public Vector GetVector(double length)
		{
			return myVc * length;
		}

		public static Vector operator *(Direction dir, double len)
		{
			return dir.GetVector(len);
		}

		public override string ToString()
		{
			return myVc.ToString();
		}

		public bool IsNull()
		{
			return Funcs2D.IsNull(myVc);
		}

		public void SetNull()
		{
			myVc.X = 0; myVc.Y = 0;
		}

		public static Direction Null { get { return new Direction(0, 0); } }

		public bool IsEqual(Direction pt, double eps)
		{
			return Funcs2D.IsEqual(pt.myVc, myVc, eps);
		}

		public bool IsEqual(Direction pt)
		{
			return IsEqual(pt, Funcs2D.Epson);
		}

		public void Trans(bool inside, ITransEngine2D te)
		{
			Direction ip = this;
			if (inside)
				te.TransTo(ip, ref ip);
			else
				te.TransFrom(ip, ref ip);
			this.myVc = ip.myVc;
		}

		public static double AngleBetween(Direction d1, Direction d2)
		{
			return Vector.AngleBetween(d1.myVc, d2.myVc);
		}
	}
}