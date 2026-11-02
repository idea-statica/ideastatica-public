using System.Runtime.InteropServices;

namespace CI.Geometry3D
{
	[Guid("827734b4-6b4d-3140-aaec-e45d86edc7af")]
	public interface IPoint3D
	{
		double X { get; set; }

		double Y { get; set; }

		double Z { get; set; }
	}
}