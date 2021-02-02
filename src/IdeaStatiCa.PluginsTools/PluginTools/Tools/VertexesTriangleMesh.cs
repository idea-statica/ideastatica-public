using System;

namespace CI.Geometry3D
{


	public class VertexesTriangleMesh : IVertexesTriangleMesh
	{

		private IPoint3D v1;
		private IPoint3D v2;
		private IPoint3D v3;

		public VertexesTriangleMesh()
		{

		}

		public VertexesTriangleMesh(IPoint3D _v1, IPoint3D _v2, IPoint3D _v3)
		{
			v1 = _v1;
			v2 = _v2;
			v3 = _v3;
		}


		public IPoint3D V1
		{
			get
			{
				return v1;
			}

			set
			{
				v1 = value;
			}
		}

		public IPoint3D V2
		{
			get
			{
				return v2;
			}

			set
			{
				v2 = value;
			}
		}

		public IPoint3D V3
		{
			get
			{
				return v3;
			}

			set
			{
				v3 = value;
			}
		}


	}

}