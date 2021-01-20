namespace IdeaRS.OpenModel.Detail
{
	/// <summary>
	/// Type of haunch on dapped end
	/// </summary>
	public enum DappedEndHaunchType
	{
		/// <summary>
		/// No haunch
		/// </summary>
		None,

		/// <summary>
		/// Left haunch
		/// </summary>
		Left,

		/// <summary>
		/// Right haunch
		/// </summary>
		Right,

		/// <summary>
		/// Both sides
		/// </summary>
		Both
	}

	/// <summary>
	/// Representation of dapped end in IDEA StatiCa Detail
	/// </summary>
	public class DappedEnd : OpenElementId
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public DappedEnd()
		{
		}

		/// <summary>
		/// Id representing geometrical parts of Detail
		/// </summary>
		public int GeomId { get; set; }

		/// <summary>
		/// Width of dapped end
		/// </summary>
		public double Width { get; set; }

		/// <summary>
		/// Height of dapped end
		/// </summary>
		public double Height { get; set; }

		/// <summary>
		/// Master component
		/// </summary>
		public ReferenceElement MasterComponent { get; set; }

		/// <summary>
		/// Insert point 0 - 1
		/// </summary>
		public int MasterPoint { get; set; }

		/// <summary>
		/// Outline of dapped end
		/// </summary>
		public ReferenceElement Outline { get; set; }

		/// <summary>
		/// Offset between MasterPoint and InsertPoint
		/// </summary>
		public Geometry3D.Vector3D Position { get; set; }

		/// <summary>
		/// Type of haunch on dapped end
		/// </summary>
		public DappedEndHaunchType DappedEndHaunch { get; set; }

		/// <summary>
		/// Haunch size of dapped end
		/// </summary>
		public double Haunch { get; set; }

		/// <summary>
		/// Rotation of dapped end
		/// </summary>
		public double Rotation { get; set; }
	}
}