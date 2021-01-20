using System.Xml.Serialization;

namespace IdeaRS.OpenModel.Concrete
{
	/// <summary>
	/// Input value type
	/// </summary>
	public enum InputValue
	{
		/// <summary>
		/// Value is calculated
		/// </summary>
		Calculated,

		/// <summary>
		/// Value is inputed by user
		/// </summary>
		UserInput,

		/// <summary>
		/// values are calculated in FEM
		/// </summary>
		CalculatedFEM
	}

	/// <summary>
	/// Types of support
	/// </summary>
	[System.Flags]
	public enum SupportType
	{
		/// <summary>
		/// Without support
		/// </summary>
		NoSupport = 1,

		/// <summary>
		/// Hinge type
		/// </summary>
		Hinge = 2,

		/// <summary>
		/// Rigid type
		/// </summary>
		Rigid = 4,

		/// <summary>
		/// Combination of hinge and nosupport
		/// </summary>
		HingeNoSupport = Hinge | NoSupport,

		/// <summary>
		/// Combination of hinge and hinge
		/// </summary>
		HingeHinge = Hinge | Hinge,

		/// <summary>
		/// Combination of rigid and nosupport
		/// </summary>
		RigidNoSupport = Rigid | NoSupport,

		/// <summary>
		/// Combination of rigid and hinge
		/// </summary>
		RigidHinge = Rigid | Hinge,

		/// <summary>
		/// Combination of rigid and rigid
		/// </summary>
		RigidRigid = Rigid | Rigid,
	}

	/// <summary>
	/// direction of imperfection
	/// </summary>
	public enum ImperfectionDirection
	{
		/// <summary>
		/// Imperfections are separately added to both directions, y and z.
		/// </summary>
		BothDirections = 0,

		/// <summary>
		/// bigger slenderness direction
		/// </summary>
		BiggerSlenderner,

		/// <summary>
		/// resultant of moments
		/// </summary>
		MomentResultant,

		/// <summary>
		/// both
		/// </summary>
		Both,

		/// <summary>
		/// Takes imperfection direction from global code setup.
		/// This value must not be used in code setup!
		/// </summary>
		FromSetup,
	}

	/// <summary>
	/// Column data base
	/// </summary>
	[XmlInclude(typeof(ColumnDataEc2))]
	public abstract class ColumnData
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ColumnData()
		{
			L = 3.0;
			L0Y = 3.0;
			L0Z = 3.0;
			FemLenghtFromFlexibleSupport = false;
			FemL0Y = double.NaN;
			FemL0Z = double.NaN;
			EffectiveLength = InputValue.Calculated;
			SupportTopY = SupportType.Hinge;
			SupportTopZ = SupportType.Hinge;
			SupportBottomY = SupportType.Hinge;
			SupportBottomZ = SupportType.Hinge;
		}

		/// <summary>
		/// Indicates, whether use user values or calculate effect of imperfections.
		/// </summary>
		public InputValue ImperfectionsInput { get; set; }

		/// <summary>
		/// Indicates, whether use user values or calculate 2'nd order effect.
		/// </summary>
		public InputValue SecondOrderEffectInput { get; set; }

		/// <summary>
		/// Gets an indication, whether imperfections and second order have to be performed perpendicular to the y axis.
		/// </summary>
		public bool CalculateY { get; set; }

		/// <summary>
		/// Gets an indication, whether imperfections and second order have to be performed perpendicular to the z axis.
		/// </summary>
		public bool CalculateZ { get; set; }

		/// <summary>
		/// Gets or sets the system length of column.
		/// </summary>
		public double L { get; set; }

		/// <summary>
		/// Gets or sets type of input effective length
		/// </summary>
		public InputValue EffectiveLength { get; set; }

		/// <summary>
		/// Gets or sets the user defined effective length of column for Y direction.
		/// If is not defined, the must be valid support data.
		/// </summary>
		public double L0Y { get; set; }

		/// <summary>
		/// Gets or sets the user defined effective length of column for Z direction.
		/// </summary>
		public double L0Z { get; set; }

		/// <summary>
		/// true if FEM length were calculated from flexible supports
		/// </summary>
		public bool FemLenghtFromFlexibleSupport { get; set; }

		/// <summary>
		/// effective length calculated in FEM for Y direction
		/// </summary>
		public double FemL0Y { get; set; }

		/// <summary>
		/// effective length calculated in FEM for Z direction
		/// </summary>
		public double FemL0Z { get; set; }

		/// <summary>
		/// Gets or sets the end support. Not necessary if effective lengths are defined.
		/// </summary>
		public SupportType SupportTopY { get; set; }

		/// <summary>
		/// Gets or sets the end support. Not necessary if effective lengths are defined.
		/// </summary>
		public SupportType SupportTopZ { get; set; }

		/// <summary>
		/// Gets or sets the end support. Not necessary if effective lengths are defined.
		/// </summary>
		public SupportType SupportBottomY { get; set; }

		/// <summary>
		/// Gets or sets the end support. Not necessary if effective lengths are defined.
		/// </summary>
		public SupportType SupportBottomZ { get; set; }

		/// <summary>
		/// Gets or sets the direction of imperfection.
		/// </summary>
		public ImperfectionDirection ImperfectionDirection { get; set; }
	}
}