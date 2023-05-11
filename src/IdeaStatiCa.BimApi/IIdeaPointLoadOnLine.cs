using IdeaRS.OpenModel.Loading;

namespace IdeaStatiCa.BimApi
{
	/// <summary>
	/// Load in specific point on line
	/// </summary>
	public interface IIdeaPointLoadOnLine : IIdeaLoading
	{
		/// <summary>
		/// Local / global
		/// </summary>
		LoadDirection Direction { get; }

		/// <summary>
		/// Force in X direction
		/// </summary>
		double Fx { get; }

		/// <summary>
		/// Force in Y direction
		/// </summary>
		double Fy { get; }

		/// <summary>
		/// Force in Z direction
		/// </summary>
		double Fz { get; }

		/// <summary>
		/// Moment about the x-axis
		/// </summary>
		double Mx { get; }

		/// <summary>
		/// Moment about the y-axis
		/// </summary>
		double My { get; }

		/// <summary>
		/// Moment about the z-axis
		/// </summary>
		double Mz { get; }

		/// <summary>
		/// Eccentricity in Y direction
		/// </summary>
		double Ey { get; }

		/// <summary>
		/// Eccentricity in Z direction
		/// </summary>
		double Ez { get; }

		/// <summary>
		/// Segment3D (or it can be also PolyLine3D in the OpenModel)
		/// </summary>
		IIdeaSegment3D Geometry { get; }

		/// <summary>
		/// Relative position on geometry line
		/// </summary>
		double RelativePosition { get; }
	}
}
