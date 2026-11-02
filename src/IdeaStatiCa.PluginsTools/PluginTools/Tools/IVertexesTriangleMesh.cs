using System.Runtime.InteropServices;

namespace CI.Geometry3D
{

	[Guid("5A5F450F-6A5C-4FAE-BC49-5A4E7E912FE5")]
	public interface IVertexesTriangleMesh
	{

		IPoint3D V1{ get; set; }

		IPoint3D V2{ get; set; }

		IPoint3D V3{ get; set; }

	}
}