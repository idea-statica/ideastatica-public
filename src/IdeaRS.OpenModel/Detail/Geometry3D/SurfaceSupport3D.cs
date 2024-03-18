using IdeaRS.OpenModel.Geometry2D;
using System.Xml.Serialization;

namespace IdeaRS.OpenModel.Detail
{
	public enum GeometryOnSurfaceType : int
	{
		/// <summary>
		/// The geometry is determined by the whole surface
		/// </summary>
		WholeSurface = 0,

		/// <summary>
		/// The geometry is determined by a polyline
		/// </summary>
		Polyline = 1,
	}

	/// <summary>
	/// Representation of Surface Support 3D in IDEA StatiCa Detail
	/// </summary>
	[XmlInclude(typeof(SurfaceSupport3D))]
	public class SurfaceSupport3D : OpenElementId
	{
		public SurfaceSupport3D()
			: base()
		{

		}

		/// <summary>
		/// Name of 3D element
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// displacement in X
		/// </summary>
		public bool X { get; set; }

		/// <summary>
		/// displacement in Y
		/// </summary>
		public bool Y { get; set; }

		/// <summary>
		/// displacement in Z
		/// </summary>
		public bool Z { get; set; }

		/// <summary>
		/// true for user stiffness
		/// </summary>
		public bool IsUserStiffnessX { get; set; }

		/// <summary>
		/// true for user stiffness
		/// </summary>
		public bool IsUserStiffnessY { get; set; }

		/// <summary>
		/// true for user stiffness
		/// </summary>
		public bool IsUserStiffnessZ { get; set; }

		/// <summary>
		/// support stiffness X
		/// </summary>
		public double StiffnessX { get; set; }

		/// <summary>
		/// support stiffness Y
		/// </summary>
		public double StiffnessY { get; set; }

		/// <summary>
		/// support stiffness Z
		/// </summary>
		public double StiffnessZ { get; set; }

		/// <summary>
		/// pressure only for Z (in local direction)
		/// </summary>
		public bool IsPressureOnlyZ { get; set; }

		/// <summary>
		/// local / global
		/// </summary>
		public IdeaRS.OpenModel.Loading.LoadDirection Direction { get; set; }

		/// <summary>
		/// Master component
		/// </summary>
		public ReferenceElement MasterComponent { get; set; }

		/// <summary>
		/// master component surface
		/// </summary>
		public int MasterSurfaceIndex { get; set; }

		/// <summary>
		/// Type of loaded geometry
		/// </summary>
		public GeometryOnSurfaceType Type { get; set; }

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
