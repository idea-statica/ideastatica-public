using IdeaRS.OpenModel.Geometry3D;
using IdeaRS.OpenModel.Loading;

namespace IdeaStatiCa.BimApi
{
	public interface IIdeaLoadOnSurface : IIdeaObject
	{
		/// <summary>
		/// Force in X direction
		/// </summary>
		double Fx { get; set; }

		/// <summary>
		/// Force in Y direction
		/// </summary>
		double Fy { get; set; }

		/// <summary>
		/// Force in Z direction
		/// </summary>
		double Fz { get; set; }

		/// <summary>
		/// Load direction LCS or GCS
		/// </summary>
		LoadDirection Direction { get; set; }

		/// <summary>
		/// Gets, sets geometry of this LoadOnSurface
		/// </summary>
		Region3D Geometry { get; set; }

		/// <summary>
		/// Item where load is applied
		/// </summary>
		IIdeaRegion3D ReferencedGeometry { get; set; }
	}
}
