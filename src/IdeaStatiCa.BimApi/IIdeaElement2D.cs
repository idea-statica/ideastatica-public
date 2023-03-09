using IdeaRS.OpenModel.Model;
using System.Collections.Generic;

namespace IdeaStatiCa.BimApi
{
	/// <summary>
	/// Representation of element2D
	/// </summary>
	public interface IIdeaElement2D : IIdeaObjectWithResults
	{
		/// <summary>
		/// Material
		/// </summary>
		IIdeaMaterial Material { get; }

		/// <summary>
		/// Geometry region of element2D
		/// </summary>
		IIdeaRegion3D GeometricRegion { get; }

		/// <summary>
		/// Thicknes of Element2D
		/// </summary>
		double Thickness { get; }

		/// <summary>
		/// Displacement in local axis Z 
		/// </summary>
		double EccentricityZ { get; }

		/// <summary>
		/// Inner lines of this region
		/// </summary>
		List<IIdeaPolyLine3D> InnerLines { get; }

		/// <summary>
		/// Inner points of this region
		/// </summary>
		List<IIdeaNode> InnerPoints { get; }

		/// <summary>
		/// Analysis type - shell, plate, wall
		/// </summary>
		Element2DType ElementType { get; }
	}
}
