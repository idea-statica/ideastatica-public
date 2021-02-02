using System;

namespace CI.Geometry3D
{


	public class IndexTriangleMesh : IIndexTriangleMesh
	{

		private int v1;
		private int v2;
		private int v3;

		public IndexTriangleMesh()
		{

		}

		public IndexTriangleMesh(int _v1, int _v2, int _v3)
		{
			v1 = _v1;
			v2 = _v2;
			v3 = _v3;
		}


		public int V1
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

		public int V2
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

		public int V3
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