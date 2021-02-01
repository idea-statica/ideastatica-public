using System;

namespace CI.Geometry3D
{

	public class BeamMesh: IBeamMesh
	{
		private IPoint3D[] vertices;
		private IIndexTriangleMesh[] indexTriangles;

		public BeamMesh()
		{

		}

		public BeamMesh(IPoint3D[] _vertices, IIndexTriangleMesh[] _indexTriangles)
		{
			vertices = _vertices;
			indexTriangles = _indexTriangles;
		}

	

		public IPoint3D[] Vertices
		{
			get
			{
				return vertices;
			}

			set
			{
				vertices = value;
			}
		}

							
		public IIndexTriangleMesh[] IndexTriangles
		{
			get
			{
				return indexTriangles;
			}

			set
			{
				indexTriangles = value;
			}
		}


	}

}