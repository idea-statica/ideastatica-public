using System.Xml.Serialization;

namespace IdeaRS.OpenModel.Concrete
{
	/// <summary>
	/// Member type
	/// </summary>
	public enum ConcreteMemberType
	{
		/// <summary>
		/// Not defined
		/// </summary>
		NoDefined = 0x0,

		/// <summary>
		/// Beam
		/// </summary>
		Beam = 0x1,

		/// <summary>
		/// Column
		/// </summary>
		Column = 0x2,

		/// <summary>
		/// Beam slab
		/// </summary>
		BeamSlab = 0x4,

		/// <summary>
		/// Hollw core slab
		/// </summary>
		HollowCoreSlab = 0x8,

		/// <summary>
		/// 2-way slab
		/// </summary>
		TwoWaySlab = 0x10,

		/// <summary>
		/// Plate
		/// </summary>
		Plate = 0x20,

		/// <summary>
		/// Wall
		/// </summary>
		Wall = 0x40,
	}

	/// <summary>
	/// Two way slab types
	/// </summary>
	public enum TwoWaySlabType
	{
		/// <summary>
		/// slab model - only bending forces + shell
		/// </summary>
		Slab,

		/// <summary>
		/// wall model - only membrane forces
		/// </summary>
		Wall,

		/// <summary>
		/// wall model - only membrane forces - detailing for deep beam
		/// </summary>
		DeepBeam,

		/// <summary>
		/// shell model - all forces - detailing for slab
		/// </summary>
		ShellAsPlate,

		/// <summary>
		/// shell model - all forces - detailing for wall
		/// </summary>
		ShellAsWall
	}

	/// <summary>
	/// Concrete member data base
	/// </summary>
	[XmlInclude(typeof(ConcreteMemberDataEc2))]
	public abstract class ConcreteMemberData : OpenAttribute
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ConcreteMemberData()
		{
			MemberType = ConcreteMemberType.Beam;
			TwoWaySlabType = TwoWaySlabType.ShellAsPlate;
			//CalculationSetup = new CalculationSetup();
		}

		/// <summary>
		/// Gets or sets the structural type of member.
		/// </summary>
		public ConcreteMemberType MemberType { get; set; }

		/// <summary>
		/// two way slab type
		/// </summary>
		public TwoWaySlabType TwoWaySlabType { get; set; }

		/// <summary>
		/// Time axis
		/// </summary>
		public TimeAxis TimeAxis { get; set; }

		/// <summary>
		/// Calculation setup
		/// </summary>
		public CalculationSetup CalculationSetup { get; set; }
	}
}