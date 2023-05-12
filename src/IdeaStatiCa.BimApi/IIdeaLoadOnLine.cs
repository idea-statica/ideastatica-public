using IdeaRS.OpenModel.Loading;

namespace IdeaStatiCa.BimApi
{
	/// <summary>
	/// Load on line
	/// </summary>
	public interface IIdeaLoadOnLine : IIdeaObject
	{
		/// <summary>
		/// Begin position on Segment3D
		/// </summary>
		double RelativeBeginPosition { get; }

		/// <summary>
		/// End position on Segment3D
		/// </summary>
		double RelativeEndPosition { get; }

		/// <summary>
		/// Eccentricity local Y on the beginning
		/// </summary>
		double ExY { get; }

		/// <summary>
		/// Eccentricity local Z on the beginning
		/// </summary>
		double ExZ { get; }

		/// <summary>
		/// Eccentricity local Y at the end
		/// </summary>
		double ExYEnd { get; }

		/// <summary>
		/// Eccentricity local Z at the end
		/// </summary>
		double ExZEnd { get; }

		/// <summary>
		/// Type of load
		/// </summary>
		LoadType Type { get; }

		/// <summary>
		/// 1=global, 0=local
		/// </summary>
		LoadDirection Direction { get; }

		/// <summary>
		/// Impulse at the begin
		/// </summary>
		LoadImpulse Bimp { get; }

		/// <summary>
		/// Impulse at the end
		/// </summary>
		LoadImpulse Eimp { get; }

		/// <summary>
		/// Segment3D (or it can be also PolyLine3D in the OpenModel)
		/// </summary>
		IIdeaSegment3D Geometry { get; }

		/// <summary>
		/// Gets, sets load projection
		/// </summary>
		LoadProjection LoadProjection { get; }
	}
}
