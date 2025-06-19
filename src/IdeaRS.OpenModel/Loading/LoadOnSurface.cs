using IdeaRS.OpenModel.Geometry3D;

namespace IdeaRS.OpenModel.Loading
{
	/// <summary>
	/// Load on surface
	/// </summary>
	[OpenModelClass("CI.StructModel.Loading.LoadOnSurface,CI.Loading", "CI.StructModel.Loading.ILoadOnSurface,CI.BasicTypes")]
	public class LoadOnSurface : OpenElementId
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public LoadOnSurface()
		{
		}

		/// <summary>
		/// Force in X direction
		/// </summary>
		public System.Double Fx { get; set; }

		/// <summary>
		/// Force in Y direction
		/// </summary>
		public System.Double Fy { get; set; }

		/// <summary>
		/// Force in Z direction
		/// </summary>
		public System.Double Fz { get; set; }

		/// <summary>
		/// Load direction LCS or GCS
		/// </summary>
		public LoadDirection Direction { get; set; }

		/// <summary>
		/// Gets, sets geometry of this LoadOnSurface
		/// </summary>
		public Region3D Geometry { get; set; }

		/// <summary>
		/// Item where load is applied
		/// </summary>
		public ReferenceElement ReferencedGeometry { get; set; }
	}
}