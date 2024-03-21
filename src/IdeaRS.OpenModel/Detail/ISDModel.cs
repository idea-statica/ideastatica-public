namespace IdeaRS.OpenModel.Detail
{

	/// <summary>
	/// Type of IDEA StatiCa Detail
	/// </summary>
	public enum ISDType
	{
		/// <summary>
		/// Wall
		/// </summary>
		Wall,

		/// <summary>
		/// Member1D
		/// </summary>
		Member1D,

		/// <summary>
		/// Frame joint
		/// </summary>
		FrameJoint,

		/// <summary>
		/// Diaphragm
		/// </summary>
		Diaphragm,

		/// <summary>
		/// Solid Block 3D
		/// </summary>
		SolidBlock,
	}

	/// <summary>
	/// Model of IDEA StatiCa Detail
	/// </summary>
	public class ISDModel : OpenElementId
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ISDModel()
		{
			Geometry = new System.Collections.Generic.List<ReferenceElement>();
			Reinforcement = new System.Collections.Generic.List<ReferenceElement>();
			Loading = new System.Collections.Generic.List<ReferenceElement>();
		}

		/// <summary>
		/// Type of IDEA StatiCa Detail
		/// </summary>
		public ISDType ISDType { get; set; }

		/// <summary>
		/// Setting
		/// </summary>
		public ISDSetting Setting { get; set; }

		/// <summary>
		/// All geometrical entities of Detail 
		/// </summary>
		public System.Collections.Generic.List<ReferenceElement> Geometry { get; set; }

		/// <summary>
		/// All reinforcement of Detail 
		/// </summary>
		public System.Collections.Generic.List<ReferenceElement> Reinforcement { get; set; }

		/// <summary>
		/// All loading entities of Detail 
		/// </summary>
		public System.Collections.Generic.List<ReferenceElement> Loading { get; set; }
	}
}
