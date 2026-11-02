namespace CI.GiCL2D
{
	public abstract class BasePolygonReader : IPolygonReader
	{
		#region IPolygonReader

		public abstract int Length { get; }

		public abstract void GetRow(int row, out double x, out double y);

		public virtual double[,] CopyTo()
		{
			var count = this.Length;
			var ret = new double[count, 2];
			for (int i = 0; i < count; i++)
			{
				GetRow(i, out ret[i, 0], out ret[i, 1]);
			}

			return ret;
		}

		#endregion IPolygonReader
	}
}