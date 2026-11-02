namespace IdeaRS.OpenModel.Concrete
{
	/// <summary>
	/// Member importance
	/// </summary>
	public enum MemberImportance
	{
		/// <summary>
		/// Major
		/// </summary>
		Major = 0,

		/// <summary>
		/// Minor
		/// </summary>
		Minor,
	}

	/// <summary>
	/// deflection calculation requirements
	/// </summary>
	public enum DeflectionRequirementEc2
	{
		/// <summary>
		/// The appearance and general utility of the structure could be impaired when the calculated
		/// sag of a beam, slab or cantilever subjected to quasi-permanent loads exceeds span/250.
		/// </summary>
		Normal,

		/// <summary>
		/// Deflections that could damage adjacent parts of the structure should be limited.
		/// </summary>
		Advanced
	}

	/// <summary>
	/// Concrete member data Ec2
	/// </summary>
	[OpenModelClass("CI.Services.MemberData.Concrete.Ec2.ConcreteMemberDataEc2,CI.ConcreteSetup")]
	public class ConcreteMemberDataEc2 : ConcreteMemberData
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ConcreteMemberDataEc2()
			: base()
		{
			CreepCoeffInfinityValue = InputValue.Calculated;
			CreepCoeffInfinity = 2.5;
			RelativeHumidity = 0.65;
			CoeffKxForWmax = 1.0;
			MemberImportance = MemberImportance.Major;
			DeflectionRequirement = DeflectionRequirementEc2.Normal;

			//ColumnData = new ColumnDataEc2();
			//ExposureClassesData = new ExposureClassesDataEc2();
			//BeamData = new BeamDataEc2();
		}

		/// <summary>
		/// Column data
		/// </summary>
		public ColumnDataEc2 ColumnData { get; set; }

		/// <summary>
		/// Exposure classes
		/// </summary>
		public ExposureClassesDataEc2 ExposureClassesData { get; set; }

		/// <summary>
		/// Beam data
		/// </summary>
		public BeamDataEc2 BeamData { get; set; }

		/// <summary>
		/// Gets or sets the surrounding relative humidity.
		/// </summary>
		public double RelativeHumidity { get; set; }

		/// <summary>
		/// type of creep coefficient
		/// </summary>
		public InputValue CreepCoeffInfinityValue { get; set; }

		/// <summary>
		/// Gets or sets the user defined creep coefficient in infinity. If null, then we calculate them.
		/// </summary>
		public double CreepCoeffInfinity { get; set; }

		/// <summary>
		/// Gets or sets the member importance.
		/// </summary>
		public MemberImportance MemberImportance { get; set; }

		/// <summary>
		///coefficient kx to increase limited concrete crack - only Dutch NAD
		/// </summary>
		public double CoeffKxForWmax { get; set; }

		/// <summary>
		/// gets or set deflection requirement for deflection calculation
		/// </summary>
		public DeflectionRequirementEc2 DeflectionRequirement { get; set; }
	}
}