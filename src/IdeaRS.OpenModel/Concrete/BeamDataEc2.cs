namespace IdeaRS.OpenModel.Concrete
{
	/// <summary>
	/// Define type of support conditions
	/// </summary>
	public enum TypeOfCalculationDeflection
	{
		/// <summary>
		/// check slenderness calculation, if rules are full filed no other calculation is done
		/// </summary>
		CheckSlenderness = 0,

		/// <summary>
		/// if check of slenderness is not ok then, directly calculation of deflection is done
		/// </summary>
		CheckSlendernessContinueDirectCalculation = 1,

		/// <summary>
		/// check of slenderness is not taken into account, directly calculation of deflection is done
		/// </summary>
		DirectCalculation = 2,

		/// <summary>
		/// both calculation is done, it is for comparative results
		/// </summary>
		BothOfThem = 3
	}

	/// <summary>
	/// Define type of support conditions
	/// </summary>
	public enum TypeOfSupportConditions
	{
		/// <summary>
		/// Non-continuous members
		/// </summary>
		NonContinuous,

		/// <summary>
		/// Continuous members
		/// </summary>
		Continuous,

		/// <summary>
		/// Supports considered fully restrained
		/// </summary>
		FullyRestrained,

		/// <summary>
		/// Bearing provided
		/// </summary>
		Bearing,

		/// <summary>
		/// Cantilever
		/// </summary>
		Cantilever,
	}

	/// <summary>
	/// Scheme
	/// </summary>
	public enum TypeDeflectionSheme
	{
		/// <summary>
		/// constant moment - single span
		/// </summary>
		DeflectionSheme_I,

		/// <summary>
		/// one single force - single span
		/// </summary>
		DeflectionSheme_II,

		/// <summary>
		/// single moment at the begging or end of the span - single span
		/// </summary>
		DeflectionSheme_III,

		/// <summary>
		/// two forces  - single span
		/// </summary>
		DeflectionSheme_IV,

		/// <summary>
		/// linear distributed loads - single span
		/// </summary>
		DeflectionSheme_V,

		/// <summary>
		/// triangle distributed loads - single span
		/// </summary>
		DeflectionSheme_VI,

		/// <summary>
		/// linear distributed loads, moments in the left and right - continues beam
		/// </summary>
		DeflectionSheme_VII,

		/// <summary>
		/// one single force - cantilever
		/// </summary>
		DeflectionSheme_VIII,

		/// <summary>
		/// triangle distributed loads - cantilever
		/// </summary>
		DeflectionSheme_IX,

		/// <summary>
		/// one single force - non continues span
		/// </summary>
		DeflectionSheme_X,

		/// <summary>
		/// trapezoidal shape of loads
		/// </summary>
		DeflectionSheme_XI
	}

	/// <summary>
	/// Beam data Ec2
	/// </summary>
	public class BeamDataEc2
	{
		/// <summary>
		/// constructor
		/// </summary>
		public BeamDataEc2()
		{
			Ln = 1.0;
			BeamSpanLength = 1.0;
			TypeOfSupportLeft = TypeOfSupportConditions.NonContinuous;
			TypeOfSupportRight = TypeOfSupportConditions.NonContinuous;
			WidthOfSupportLeft = 0.4;
			WidthOfSupportRight = 0.4;
			TypeOfCalculation = TypeOfCalculationDeflection.CheckSlenderness;
			DeflectionSheme = TypeDeflectionSheme.DeflectionSheme_V;
			CheckLimitDeflection = true;
			IsUserLimitDeflection = false;
			UserLimitDeflection = double.NaN;
			CheckLimitDeflRheoEffects = false;
			UserLimitDeflRheoEffects = double.NaN;
			IsAbsValueLimitDeflection = false;
			RequiredCamber = 0.0;
			IsYDirectionData = false;
		}

		/// <summary>
		/// there are several types of calculation
		/// </summary>
		public TypeOfCalculationDeflection TypeOfCalculation { get; set; }

		/// <summary>
		/// Length of beam
		/// </summary>
		public double Ln { get; set; }

		/// <summary>
		/// beam span lengrh - theoretical support distance
		/// </summary>
		public double BeamSpanLength { get; set; }

		/// <summary>
		/// Type of support on the left side
		/// </summary>
		public TypeOfSupportConditions TypeOfSupportLeft { get; set; }

		/// <summary>
		/// Type of support on the right side
		/// </summary>
		public TypeOfSupportConditions TypeOfSupportRight { get; set; }

		/// <summary>
		/// Width of support on the left side
		/// </summary>
		public double WidthOfSupportLeft { get; set; }

		/// <summary>
		/// Width of support on the right side
		/// </summary>
		public double WidthOfSupportRight { get; set; }

		/// <summary>
		/// type of span loads
		/// </summary>
		public TypeDeflectionSheme DeflectionSheme { get; set; }

		/// <summary>
		/// indicator for check of limit deflection for beam span
		/// </summary>
		public bool CheckLimitDeflection { get; set; }

		/// <summary>
		/// true for user value of limit deflection (both - total + rheological)
		/// </summary>
		public bool IsUserLimitDeflection { get; set; }

		/// <summary>
		/// user value of limit deflection - denominator - 1 / value
		/// </summary>
		public double UserLimitDeflection { get; set; }

		/// <summary>
		/// true for check of limit deflection for beam span from rheological effects of variable loads
		/// </summary>
		public bool CheckLimitDeflRheoEffects { get; set; }

		/// <summary>
		/// user value of limit deflection from rheological effects of variable loads - denominator - 1 / value
		/// </summary>
		public double UserLimitDeflRheoEffects { get; set; }

		/// <summary>
		/// true for absolute user value of limit deflection (both - total + rheological)
		/// </summary>
		public bool IsAbsValueLimitDeflection { get; set; }

		/// <summary>
		/// required camber
		/// </summary>
		public double RequiredCamber { get; set; }

		/// <summary>
		///  true for deflection data in Y direction
		/// </summary>
		public bool IsYDirectionData { get; set; }
	}
}