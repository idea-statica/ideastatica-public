using System.Collections.Generic;

namespace IdeaRS.OpenModel.Geometry3D
{
	/// <summary>
	/// Represents a region in three-dimensional space included outline (border) and openings.
	/// </summary>
	[OpenModelClass("CI.Geometry3D.Region3D,CI.Geometry3D", "CI.Geometry3D.IRegion3D,CI.BasicTypes")]
	public class Region3D : OpenElementId
	{

		/// <summary>
		/// Constructor
		/// </summary>
		public Region3D()
		{
			Openings = new List<ReferenceElement>();
		}

		/// <summary>
		/// Gets or sets the reference to <see cref="IdeaRS.OpenModel.Geometry3D.PolyLine3D "/> curve of Region3D.
		/// </summary>
		public ReferenceElement Outline { get; set; }

		/// <summary>
		/// Gets or sets the list of references to <see cref="IdeaRS.OpenModel.Geometry3D.PolyLine3D "/>openning curve in the Region3D.
		/// </summary>
		public List<ReferenceElement> Openings { get; set; }

		/// <summary>
		/// Local coordinate system
		/// </summary>
		[OpenModelProperty("LCS")]
		public CoordSystem LocalCoordinateSystem { get; set; }
	}
}