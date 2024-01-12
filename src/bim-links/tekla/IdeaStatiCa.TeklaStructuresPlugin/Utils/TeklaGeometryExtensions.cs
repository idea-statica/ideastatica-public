using Tekla.Structures.Geometry3d;
using WM = System.Windows.Media.Media3D;

namespace IdeaStatica.TeklaStructuresPlugin.Utils
{
	public static class TeklaGeometryExtensions
	{
		internal static WM.Point3D ToMediaPoint(this Point src)
		{
			return new WM.Point3D(src.X, src.Y, src.Z);
		}

		internal static WM.Vector3D ToMediaVector(this Vector src)
		{
			return new WM.Vector3D(src.X, src.Y, src.Z);
		}
	}
}

