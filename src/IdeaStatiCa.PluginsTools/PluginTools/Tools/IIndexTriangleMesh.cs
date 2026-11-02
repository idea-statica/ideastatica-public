using System.Runtime.InteropServices;

namespace CI.Geometry3D
{
	
	[Guid("3A18ED7B-7192-4F2C-A35A-001E61DC4E80")]
	public interface IIndexTriangleMesh
	{

		int V1{ get; set; }

		int V2{ get; set; }
		
		int V3{ get; set; }

	}
}