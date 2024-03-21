namespace IdeaRS.OpenModel.Detail.Loading
{
	/// <summary>
	/// Representation of Point Load 3D in IDEA StatiCa Detail
	/// </summary>
	public class PointLoad3D : LoadBase
	{
		public PointLoad3D() 
			: base ()
		{
		}

		/// <summary>
		/// Name of 3D element
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// point load radius - only if point load is not assigned to master device
		/// </summary>
		public double RadiusOfLoad { get; set; }

		/// <summary>
		/// force in X
		/// </summary>
		public double Fx { get; set; }

		/// <summary>
		/// force in Y
		/// </summary>
		public double Fy { get; set; }

		/// <summary>
		/// force in Z
		/// </summary>
		public double Fz { get; set; }

		/// <summary>
		/// point support position X
		/// </summary>
		public double PositionX { get; set; }

		/// <summary>
		/// point support position Y
		/// </summary>
		public double PositionY { get; set; }

		/// <summary>
		/// point support position Z
		/// </summary>
		public double PositionZ { get; set; }

		/// <summary>
		/// angle of load
		/// </summary>
		public double Angle { get; set; }

		/// <summary>
		/// Gets or sets the master component
		/// </summary>
		public ReferenceElement MasterComponent { get; set; }

		/// <summary>
		/// Surface index of solid block ;
		/// SurfaceIndex set on -1 when MasterComponent is not SB
		/// </summary>
		public int SurfaceIndex { get; set; }

		/// <summary>
		/// Edge index of solid block ;
		/// EdgeIndex set on -1 when MasterComponent is not SB
		/// </summary>
		public int EdgeIndex { get; set; }

		/// <summary>
		/// direction type of load - local/global
		/// </summary>
		public IdeaRS.OpenModel.Loading.LoadDirection Direction { get; set; }
	}
}
