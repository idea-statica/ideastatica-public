using System;

namespace CI.GiCL2D
{
	internal class BoxArrayDouble2 : BasePolygonReader
	{
		private double[,] myPoly;

		public BoxArrayDouble2(double[,] myPoly)
		{
			this.myPoly = myPoly;
		}

		#region BasePolygonReader

		public override int Length
		{
			get { throw new NotImplementedException(); }
		}

		public override void GetRow(int row, out double x, out double y)
		{
			throw new NotImplementedException();
		}

		#endregion BasePolygonReader
	}
}