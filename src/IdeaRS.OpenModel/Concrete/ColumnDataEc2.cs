using System;

namespace IdeaRS.OpenModel.Concrete
{
	/// <summary>
	/// Effect concidered types
	/// </summary>
	public enum EffectConsideredType
	{
		/// <summary>
		/// Isolated system
		/// </summary>
		IsolatedMember,

		/// <summary>
		/// Bracing system
		/// </summary>
		BracingSystem
	}

	/// <summary>
	/// The type of method for calculation second order effect.
	/// </summary>
	public enum SecondOrderEffectMethodEc2
	{
		/// <summary>
		/// The following model may be used to estimate the nominal stiffness of slender compression
		/// members with arbitrary cross section.
		/// </summary>
		NominalStiffness,

		/// <summary>
		/// The method based on nominal curvature.
		/// This method is primarily suitable for isolated members with constant
		/// normal force and a defined effective length.
		/// </summary>
		NominalCurvature,
	}

	/// <summary>
	/// Define type of c0 value
	/// </summary>
	public enum ValueTypec0
	{
		/// <summary>
		/// Value is inputed by user
		/// </summary>
		UserDefined,

		/// <summary>
		/// Value depends on constant distribution of moment
		/// </summary>
		ConstantDistributionMoment,

		/// <summary>
		/// Value depends on parabolic distribution of moment
		/// </summary>
		ParabolicDistributionMoment,

		/// <summary>
		/// Value depends on triangular distribution of moment
		/// </summary>
		TriangularDistributionMoment
	}

	/// <summary>
	/// Define type of c value
	/// </summary>
	public enum ValueTypec
	{
		/// <summary>
		/// Value is inputed by user
		/// </summary>
		UserDefined,

		/// <summary>
		/// Value depends on constant curvature distributio
		/// </summary>
		ConstantCurvatureDistribution,

		/// <summary>
		/// Value depends on sinusoidal curvature distribution
		/// </summary>
		SinusoidalCurvatureDistribution,
	}

	/// <summary>
	/// Column data Ec2
	/// </summary>
	public class ColumnDataEc2 : ColumnData
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ColumnDataEc2()
			: base()
		{
			NumberOfVerticalMembersY = 1;
			NumberOfVerticalMembersZ = 1;
			TotalL = 20.0;

			BracedY = false;
			BracedZ = false;

			GeometricImperfectionsULS = true;
			GeometricImperfectionsSLS = false;
			EffectConsidered = EffectConsideredType.IsolatedMember;

			Calculation2ndOrderEffect = true;
			SecondOrderEffectMethod = SecondOrderEffectMethodEc2.NominalCurvature;

			ValueTypeOfc0Y = ValueTypec0.UserDefined;
			UserValuec0Y = Math.Pow(Math.PI, 2.0);
			ValueTypeOfc0Z = ValueTypec0.UserDefined;
			UserValuec0Z = Math.Pow(Math.PI, 2.0);
			ValueTypeOfcY = ValueTypec.UserDefined;
			UserValuecY = Math.Pow(Math.PI, 2.0);
			ValueTypeOfcZ = ValueTypec.UserDefined;
			UserValuecZ = Math.Pow(Math.PI, 2.0);
		}

		/// <summary>
		/// Braced structure Y-Y
		/// </summary>
		public bool BracedY { get; set; }

		/// <summary>
		/// Braced structure Z-Z
		/// </summary>
		public bool BracedZ { get; set; }

		/// <summary>
		/// Use calculation geometric imperfections for ULS
		/// </summary>
		public bool GeometricImperfectionsULS { get; set; }

		/// <summary>
		/// Use calculation geometric imperfections for SLS
		/// </summary>
		public bool GeometricImperfectionsSLS { get; set; }

		/// <summary>
		/// Effect considered
		/// </summary>
		public EffectConsideredType EffectConsidered { get; set; }

		/// <summary>
		/// Gets or sets the total length of building
		/// </summary>
		public double TotalL { get; set; }

		/// <summary>
		/// Gets or sets the number of vertical member in Y direction.
		/// </summary>
		public int NumberOfVerticalMembersY { get; set; }

		/// <summary>
		/// Gets or sets the number of vertical member in Z direction.
		/// </summary>
		public int NumberOfVerticalMembersZ { get; set; }

		/// <summary>
		/// Use calculation of 2nd order effect
		/// </summary>
		public bool Calculation2ndOrderEffect { get; set; }

		/// <summary>
		/// Type of calculation method of 2nd order effect
		/// </summary>
		public SecondOrderEffectMethodEc2 SecondOrderEffectMethod { get; set; }

		/// <summary>
		/// Type of c0 value
		/// </summary>
		public ValueTypec0 ValueTypeOfc0Y { get; set; }

		/// <summary>
		/// c0 user value - for nominal stiffness
		/// </summary>
		public double UserValuec0Y { get; set; }

		/// <summary>
		/// Type of c0 value
		/// </summary>
		public ValueTypec0 ValueTypeOfc0Z { get; set; }

		/// <summary>
		/// c0 user value - for nominal stiffness
		/// </summary>
		public double UserValuec0Z { get; set; }

		/// <summary>
		/// Type of c value - for nominal curvature
		/// </summary>
		public ValueTypec ValueTypeOfcY { get; set; }

		/// <summary>
		/// c user value - for nominal stiffness
		/// </summary>
		public double UserValuecY { get; set; }

		/// <summary>
		/// Type of c value - for nominal curvature
		/// </summary>
		public ValueTypec ValueTypeOfcZ { get; set; }

		/// <summary>
		/// c user value - for nominal stiffness
		/// </summary>
		public double UserValuecZ { get; set; }
	}
}