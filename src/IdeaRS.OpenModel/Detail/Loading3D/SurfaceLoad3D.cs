using IdeaRS.OpenModel.Geometry2D;

namespace IdeaRS.OpenModel.Detail.Loading
{
	/// <summary>
	/// Representation of Surface Load 3D in IDEA StatiCa Detail
	/// </summary>
	public class SurfaceLoad3D : LoadBase
	{
		public SurfaceLoad3D()
			: base()
		{
		
		}

		/// <summary>
		/// Name of 3D element
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The impulse is an intensity defined by force per unit area
		/// </summary>
		public IdeaRS.OpenModel.Geometry3D.Vector3D Impulse { get; set; }

		/// <summary>
		/// direction type of load - local/global
		/// </summary>
		public IdeaRS.OpenModel.Loading.LoadDirection Direction { get; set; }

		/// <summary>
		/// Gets or sets the master component
		/// </summary>
		public ReferenceElement MasterComponent { get; set; }

		/// <summary>
		/// master component surface
		/// </summary>
		public int MasterSurfaceIndex { get; set; }

		/// <summary>
		/// Type of loaded geometry
		/// </summary>
		public GeometryOnSurfaceType LoadedGeometryType { get; set; }

		/// <summary>
		/// General shape - for LoadedGeometryType == GeometryOnSurfaceType.Polyline
		/// </summary>
		public PolyLine2D GeneralShape { get; set; }

		/// <summary>
		/// Transaction along the local X axis
		/// </summary>
		public double TranslationX { get; set; }

		/// <summary>
		/// Transaction along the local Y axis
		/// </summary>
		public double TranslationY { get; set; }

		/// <summary>
		/// Rotation about the local Z axis
		/// </summary>
		public double RotationAboutZ { get; set; }
	}
}
