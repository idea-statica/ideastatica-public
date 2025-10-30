using System.Xml.Serialization;

namespace IdeaRS.OpenModel.Detail
{
	public enum CastInPlateFastenerType : int
	{
		/// <summary>
		/// Reinforcement
		/// </summary>
		Reinforcement,

		/// <summary>
		/// HeadedStud
		/// </summary>
		CastInHeadedStud
	}

	public enum CastInPlateFastenerShape : int
	{
		/// <summary>
		/// Straight
		/// </summary>
		Straight,

		/// <summary>
		/// L shape
		/// </summary>
		LShape,

		/// <summary>
		/// U shape
		/// </summary>
		UShape
	}


	[XmlInclude(typeof(CastInPlateFastenerGroup))]
	[XmlInclude(typeof(CastInPlateFastenerSingle))]
	public abstract class CastInPlateFastenerBase : OpenObject
	{
		protected CastInPlateFastenerBase()
		{

		}

		/// <summary>
		/// Element Id
		/// </summary>
		public System.Int32 Id { get; set; }

		/// <summary>
		/// Name of 3D element
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// anchor type
		/// </summary>
		public CastInPlateFastenerType FastenerType { get; set; }

		/// <summary>
		/// shape
		/// </summary>
		public CastInPlateFastenerShape Shape { get; set; }

		/// <summary>
		/// Diameter of fastener
		/// </summary>
		public double Diameter { get; set; }

		///// <summary>
		///// diameter name - pokud neni naplneny, sestavi se z convertoru
		///// </summary>
		//public string DiameterName { get; set; }

		///// <summary>
		///// Input type for diameter of reinforcement bar
		///// </summary>
		//public DiameterInputType DiameterInputType { get; set; }

		/// <summary>
		/// Hanging name
		/// </summary>
		public ReferenceElement Material { get; set; }


		public LongReinfEndType AnchorageType { get; set; }

		/// <summary>
		/// true for user diameter of mandrel
		/// </summary>
		public bool UserDiameterOfMandrel { get; set; }

		/// <summary>
		/// user diameter of mandrel value
		/// </summary>
		public double DiameterOfMandrel { get; set; }

		/// <summary>
		/// Length of upper part of anchor
		/// </summary>
		public double LengthUp { get; set; }

		/// <summary>
		/// Length of down part of anchor
		/// </summary>
		public double LengthDown { get; set; }

		/// <summary>
		/// Rotation of fastener
		/// </summary>
		public double Rotation { get; set; }

		/// <summary>
		/// HeadedStudHeadDiameter
		/// </summary>
		public double HeadedStudHeadDiameter { get; set; }

		/// <summary>
		/// Master component
		/// </summary>
		public ReferenceElement MasterComponent { get; set; }

		/// <summary>
		/// Axial forces are transferred
		/// </summary>
		public bool TransferOfAxialForces { get; set; }

		/// <summary>
		/// Shear forces are transferred
		/// </summary>
		public bool TransferOfShear { get; set; }
	}
}
