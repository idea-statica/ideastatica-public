using IdeaRS.OpenModel.Geometry3D;
using System.Collections.Generic;

namespace IdeaRS.OpenModel.Model
{

	/// <summary>
	/// Analysis Type of 2D IOM element
	/// </summary>
	public enum Element2DType
	{
		/// <summary>
		/// Shell
		/// </summary>
		Shell,

		/// <summary>
		/// Slab
		/// </summary>
		Slab,

		/// <summary>
		/// Wall
		/// </summary>
		Wall,
	}

	/// <summary>
	/// Representation of element2D
	/// </summary>
	[OpenModelClass("CI.StructModel.Structure.Element2D,CI.StructuralElements", "CI.StructModel.Structure.IElement2D,CI.BasicTypes")]
	public class Element2D : OpenElementId
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public Element2D()
		{
			InnerLines = new List<ReferenceElement>();
			InnerPoints = new List<ReferenceElement>();
		}

		/// <summary>
		/// Name of 2D element
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Material
		/// </summary>
		public ReferenceElement Material { get; set; }

		/// <summary>
		/// Geometry region of element2D
		/// </summary>
		public ReferenceElement GeometricRegion { get; set; }

		/// <summary>
		/// Thicknes of Element2D
		/// </summary>
		public double Thickness { get; set; }

		/// <summary>
		/// Displacement in local axis Z 
		/// </summary>
		public double EccentricityZ { get; set; }

		/// <summary>
		/// Inner lines of this region
		/// </summary>
		public List<ReferenceElement> InnerLines { get; set; }

		/// <summary>
		/// Inner points of this region
		/// </summary>
		public List<ReferenceElement> InnerPoints { get; set; }

		/// <summary>
		/// Analysis type - shell, plate, wall
		/// </summary>
		public Element2DType ElementType { get; set; }
	}
}
