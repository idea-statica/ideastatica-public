using System.Collections.Generic;

namespace IdeaRS.OpenModel.Geometry3D
{
	/// <summary>
	/// Represents a polyline in three-dimensional space.
	/// </summary>
	[OpenModelClass("CI.Geometry3D.PolyLine3D,CI.Geometry3D", "CI.Geometry3D.IPolyLine3D,CI.BasicTypes")]
	public class PolyLine3D : OpenElementId
	{
		/// <summary>
		/// Contructor
		/// </summary>
		public PolyLine3D()
		{
			Segments = new List<ReferenceElement>();
		}

		/// <summary>
		/// Gets or sets list of references to <see cref="IdeaRS.OpenModel.Geometry3D.Segment3D "/> of <c>PolyLine3D</c>.
		/// </summary>
		public List<ReferenceElement> Segments { get; set; }
	}
}