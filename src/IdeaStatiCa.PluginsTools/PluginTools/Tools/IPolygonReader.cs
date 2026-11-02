
namespace CI.GiCL2D
{
	public interface IPolygonReader
	{
		int Length { get; }
		void GetRow(int row, out double x, out double y);
		double[,] CopyTo();
	}
}
