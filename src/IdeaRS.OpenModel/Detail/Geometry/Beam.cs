namespace IdeaRS.OpenModel.Detail
{
	/// <summary>
	/// Representation of 1D member in IDEA StatiCa Detail
	/// </summary>
	public class Beam : Model.Element1D, IGeometryPart
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public Beam() : base()
		{
			Edges = new System.Collections.Generic.List<ReferenceElement>();
		}

		/// <summary>
		/// Id representing geometrical parts of Detail
		/// </summary>
		public int GeomId { get; set; }

		/// <summary>
		/// Master component
		/// </summary>
		public ReferenceElement MasterComponent { get; set; }

		/// <summary>
		/// Master point 0 - 9
		/// </summary>
		public int MasterPoint { get; set; }

		/// <summary>
		/// Insert point 0 - 9
		/// </summary>
		public int InsertPoint { get; set; }

		/// <summary>
		/// Lines of Element2D including edges, which are created by opening
		/// </summary>
		public System.Collections.Generic.List<ReferenceElement> Edges { get; set; }

		/// <summary>
		/// Offset between MasterPoint and InsertPoint
		/// If MasterPoint is null, position is from origin of coordinate system
		/// </summary>
		public Geometry3D.Vector3D Position { get; set; }
	}
}