using IdeaRS.OpenModel.Concrete;
using System;

namespace IdeaRS.OpenModel
{
	/// <summary>
	/// WeldEvaluation
	/// </summary>
	[Obsolete("Not used anymore")]
	public enum WeldEvaluation
	{
		/// <summary>
		/// Check is calculated in element with max force in weld (too conservative)
		/// </summary>
		MaxForceElement = 0,

		/// <summary>
		/// Check is calculated as resultant of all forces in weld
		/// </summary>
		ForceResultant = 1,

		/// <summary>
		/// Plasticity in welds is taken into account in AM
		/// </summary>
		ApplyPlasticWelds = 4,
	}

	/// <summary>
	/// CRT IS check
	/// </summary>
	public enum CrtCompCheckIS
	{
		/// <summary>
		/// IS check
		/// </summary>
		IS800_Cl_7_4,

		/// <summary>
		/// IS check
		/// </summary>
		IS456_Cl_34_4
	}

	/// <summary>
	/// Types of cone breakout checks
	/// </summary>
	public enum ConeBreakoutCheckType
	{
		/// <summary>
		/// Both tension and shear cone breakout checks are prerformed
		/// </summary>
		Both,

		/// <summary>
		/// Only tension cone breakout checks are prerformed
		/// </summary>
		Tension,

		/// <summary>
		/// Only shear cone breakout checks are prerformed
		/// </summary>
		Shear,

		/// <summary>
		/// None of cone breakout checks are prerformed
		/// </summary>
		None
	}

	/// <summary>
	/// ConnectionSetup
	/// </summary>
	public class ConnectionSetup
	{
		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		public ConnectionSetup()
		{
			MinSize = 0.01;
			MaxSize = 0.05;
			NumElement = 8;
			NumElementRhs = 16;
			NumElementPlate = 20;
			WarnCheckLevel = 0.95;
			OptimalCheckLevel = 0.6;
			LimitPlasticStrain = 0.05;
			WarnPlasticStrain = 0.03;
			DivisionOfSurfaceOfCHS = 24;
			DivisionOfArcsOfRHS = 3;
			StopAtLimitStrain = false;
			AnchorLengthForStiffness = 8;
			JointBetaFactor = 0.67;
			NumberIterations = 15;
			EffectiveAreaStressCoeff = 0.1;
			EffectiveAreaStressCoeffAISC = 0.1;
			FrictionCoefficient = 0.25;
			Mdiv = 3;
			GammaInst = 1.2;
			GammaC = 1.5;
			MemberLengthRatio = 2.0;
			CheckDetailing = false;
			BracedSystem = false;
			DistanceBetweenBolts = 2.2;
			DistanceBetweenBoltsEdge = 1.2;
			DistanceDiameterBetweenBP = 4.0;

			ApplyConeBreakoutCheck = ConeBreakoutCheckType.Both;
			BearingAngle = 26.57 * Math.PI / 180;
			DecreasingFtrd = 0.85;
			GammaM3 = 1.25;
			FrictionCoefficientPbolt = 0.3;
			ExtensionLengthRationOpenSections = 0.5;
			ExtensionLengthRationCloseSections = 0.5;
			FactorPreloadBolt = 0.0;
			BaseMetalCapacity = true;
			AlphaCC = 1.0;
			LimitDeformation = 0.03;
			LimitDeformationCheck = false;
			AnalysisGNL = true;
			AnalysisAllGNL = false;
			DeformationBoltHole = true;
			CondensedElementLengthFactor = 1.0;
		}

		/// <summary>
		/// Inicialize by setted code
		/// </summary>
		public void InitByCode()
		{
			if (SteelSetup != null)
			{
				FrictionCoefficientPbolt = SteelSetup.FrictionCoefficientPboltDefault();
			}
		}

		#endregion Constructors

		/// <summary>
		/// Steel Setup
		/// </summary>
		public SteelSetup SteelSetup { get; set; }

		/// <summary>
		/// Concrete setup
		/// </summary>
		[Obsolete]
		public ConcreteSetup ConcreteSetup { get; set; }

		/// <summary>
		/// Stop analysis when the limit strain is reached.
		/// </summary>
		public bool StopAtLimitStrain { get; set; }

		/// <summary>
		/// Method of evaluation of stress in welds
		/// </summary>
		[Obsolete("Not used anymore")]
		public WeldEvaluation WeldEvaluationData { get; set; }

		/// <summary>
		/// Perform check of bolt positions
		/// </summary>
		public bool CheckDetailing { get; set; }

		/// <summary>
		/// Apply cone breakout check
		/// </summary>
		public ConeBreakoutCheckType ApplyConeBreakoutCheck { get; set; }

		/// <summary>
		/// Pretension force fpc = k * fub * As
		/// </summary>
		[Obsolete("Use FactorPreloadBolt instead")]
		public double PretensionForceFpc { get; set; }

		/// <summary>
		/// Partial safety factor of instalation safety
		/// </summary>
		public double GammaInst { get; set; }

		/// <summary>
		/// Partial safety factor of concrete
		/// </summary>
		public double GammaC { get; set; }

		/// <summary>
		/// Preloaded bolts safety factor
		/// </summary>
		public double GammaM3 { get; set; }

		/// <summary>
		/// Length of anchor to define the anchor stiffness in analysis model, as a multiple of anchor diameter (E A /n * [d])
		/// </summary>
		public int AnchorLengthForStiffness { get; set; }

		/// <summary>
		/// Joint coefficient βj - Used for Fjd calculation
		/// </summary>
		public double JointBetaFactor { get; set; }

		/// <summary>
		/// Effective area is taken from intersection of stress area and area of joined items according to EN1993-1-8 art. 6.2.5
		/// </summary>
		public double EffectiveAreaStressCoeff { get; set; }

		/// <summary>
		/// Effective area stress coefficient - Concrete loaded area: Stress cut-off is set for AISC
		/// </summary>
		public double EffectiveAreaStressCoeffAISC { get; set; }

		/// <summary>
		/// Coefficient of friction between base plate and concrete block
		/// </summary>
		public double FrictionCoefficient { get; set; }

		/// <summary>
		/// Limit of plastic strain used in 2D plate element check
		/// </summary>
		public double LimitPlasticStrain { get; set; }

		/// <summary>
		/// Limit deformation on closed sections 
		/// </summary>
		public double LimitDeformation { get; set; }

		/// <summary>
		/// Limit deformation on closed sections check or not
		/// </summary>
		public bool LimitDeformationCheck { get; set; }

		/// <summary>
		/// Analysis with GNL
		/// </summary>
		public bool AnalysisGNL { get; set; }

		/// <summary>
		/// Analysis with All GNL
		/// </summary>
		public bool AnalysisAllGNL { get; set; }

		/// <summary>
		/// Warning plastic strain
		/// </summary>
		public double WarnPlasticStrain { get; set; }

		/// <summary>
		/// Warning check level
		/// </summary>
		public double WarnCheckLevel { get; set; }

		/// <summary>
		/// Optimal check level
		/// </summary>
		public double OptimalCheckLevel { get; set; }

		/// <summary>
		/// Limit distance between bolts as a multiple of bolt diameter
		/// </summary>
		public double DistanceBetweenBolts { get; set; }

		/// <summary>
		/// Anchor pitch
		/// </summary>
		public double DistanceDiameterBetweenBP { get; set; }

		/// <summary>
		/// Limit distance between bolt and plate edge as a multiple of bolt diameter
		/// </summary>
		public double DistanceBetweenBoltsEdge { get; set; }

		/// <summary>
		/// Load distribution angle of concrete block in calculation of factor Kj 
		/// </summary>
		public double BearingAngle { get; set; }

		/// <summary>
		/// Decreasing Ftrd of anchors. Worse quality influence
		/// </summary>
		public double DecreasingFtrd { get; set; }

		/// <summary>
		/// Consider the frame system as braced for stiffness calculation. Braced system reduces horizontal displacements.
		/// </summary>
		public bool BracedSystem { get; set; }

		/// <summary>
		/// Apply bearing check including αb
		/// </summary>
		[Obsolete("Not used anymore")]
		public bool BearingCheck { get; set; }

		/// <summary>
		/// Apply βp influence in bolt shear resistance. ΕΝ 1993-1-8 chapter 3.6.1 (12)
		/// </summary>
		public bool ApplyBetapInfluence { get; set; }

		/// <summary>
		/// A multiple of cross-section height to determine the default length of member
		/// </summary>
		public double MemberLengthRatio { get; set; }

		/// <summary>
		/// Number of straight lines to substitute circle of circular tube in analysis model
		/// </summary>
		public int DivisionOfSurfaceOfCHS { get; set; }

		/// <summary>
		/// Number of straight lines to substitute corner arc of rectangular tubes in analysis model
		/// </summary>
		public int DivisionOfArcsOfRHS { get; set; }

		/// <summary>
		/// Ratio of length of decisive plate edge and Elements on edge count determines the average size of mesh element
		/// </summary>
		public int NumElement { get; set; }

		/// <summary>
		/// More iterations helps to find better solutions in contact elements but increases calculation time
		/// </summary>
		public int NumberIterations { get; set; }

		/// <summary>
		/// Number of iteration steps to evaluate analysis divergence
		/// </summary>
		public int Mdiv { get; set; }

		/// <summary>
		/// Minimal size of generated finite mesh element
		/// </summary>
		public double MinSize { get; set; }

		/// <summary>
		/// Maximal size of generated finite mesh element
		/// </summary>
		public double MaxSize { get; set; }

		/// <summary>
		/// Number of mesh elements in RHS height
		/// </summary>
		public int NumElementRhs { get; set; }

		/// <summary>
		/// Number of mesh elements on plates
		/// </summary>
		public int NumElementPlate { get; set; }


		/// <summary>
		/// True if rigid base plate is considered
		/// </summary>
		public bool RigidBP { get; set; }

		/// <summary>
		/// Long-term effect on fcd
		/// </summary>
		public double AlphaCC { get; set; }

		/// <summary>
		/// True if cracked concrete is considered
		/// </summary>
		public bool CrackedConcrete { get; set; }

		/// <summary>
		/// True if developed fillers is considered
		/// </summary>
		[Obsolete("Not used anymore")]
		public bool DevelopedFillers { get; set; }

		/// <summary>
		/// True if bolt hole deformation is considered 
		/// </summary>
		public bool DeformationBoltHole { get; set; }

		/// <summary>
		/// ExtensionLengthRationOpenSections
		/// </summary>
		public double ExtensionLengthRationOpenSections { get; set; }

		/// <summary>
		/// ExtensionLengthRationCloseSections
		/// </summary>
		public double ExtensionLengthRationCloseSections { get; set; }

		/// <summary>
		/// FactorPreloadBolt
		/// </summary>
		public double FactorPreloadBolt { get; set; }

		/// <summary>
		/// BaseMetalCapacity
		/// </summary>
		public bool BaseMetalCapacity { get; set; }

		/// <summary>
		/// ApplyBearingCheck
		/// </summary>
		[Obsolete("Not used anymore")]
		public bool ApplyBearingCheck { get; set; }

		/// <summary>
		/// Friction factor of slip-resistant joint
		/// </summary>
		public double FrictionCoefficientPbolt { get; set; }

		/// <summary>
		/// Crt check type
		/// </summary>
		public CrtCompCheckIS CrtCompCheckIS { get; set; }

		/// <summary>
		/// Max value of bolt grip IND
		/// </summary>
		public double BoltMaxGripLengthCoeff { get; set; }

		/// <summary>
		/// Fatigue section Offset = FatigueSectionOffset x Legsize
		/// </summary>
		public double FatigueSectionOffset { get; set; }

		/// <summary>
		/// Condensed element length factor (CEF). Condensed beam legth = maxCssSize * CEF
		/// </summary>
		public double CondensedElementLengthFactor { get; set; }

		/// <summary>
		/// Partial safety factor for Horizontal tying
		/// </summary>
		[Obsolete("Use GammaMu from SteelSetup instead")]
		public double GammaMu { get; set; }

		/// <summary>
		/// Limit plastic strain for high strength steel
		/// </summary>
		public double HssLimitPlasticStrain { get; set; }
	}
}