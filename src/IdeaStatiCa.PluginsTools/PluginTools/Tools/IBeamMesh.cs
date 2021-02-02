using System.Runtime.InteropServices;

namespace CI.Geometry3D
{
	
	[Guid("135006BA-8041-44A5-A8A7-8BB2BA8F42DF")]
	public interface IBeamMesh
	{

		IPoint3D[] Vertices { get; set; }
				
		IIndexTriangleMesh[] IndexTriangles{ get; set; }

	}
}