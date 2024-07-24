using IdeaRS.OpenModel.Concrete;
using System;

namespace IdeaRS.OpenModel
{
	/// <summary>
	/// WeldEvaluation
	/// </summary>
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

		///// <summary>
		///// Check is calculated as resultant of all forces in weld and all welds of uncoiled beams
		///// </summary>
		//[Obsolete]
		//ForceResultantFullUncoiled = 2,

		///// <summary>
		///// Check is calculated as LIN interpolation
		///// </summary>
		//[Obsolete]
		//ForceResultantLinear = 3,

		/// <summary>
		/// Plasticity in welds is taken into account in AM
		/// </summary>
		ApplyPlasticWelds = 4,
	}

	///// <summary>
	///// Welding types defined in SP16-Table 39
	///// </summary>
	//public enum WeldingTypeSNIP
	//{
	//    /// <summary>
	//    /// Manual welding
	//    /// </summary>
	//    Manual,

	//    /// <summary>
	//    /// Manual welding using rod solid cross-section with diameter less than 1.4mm
	//    /// </summary>
	//    ManualSmallRodDiam,

	//    /// <summary>
	//    /// Automatic and machine welding
	//    /// </summary>
	//    AutomaticAndMachine,

	//    /// <summary>
	//    /// Automatic welding
	//    /// </summary>
	//    Automatic
	//}

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
			//ConcentrationFactor = 2;
			JointBetaFactor = 0.67;
			NumberIterations = 15;
			EffectiveAreaStressCoeff = 0.1;
			EffectiveAreaStressCoeffAISC = 0.1;
			FrictionCoefficient = 0.25;
			Mdiv = 3;
			WeldEvaluationData = WeldEvaluation.ForceResultant;
			//AddBoreHoles = true;
			//SizeRefinement = 2.0;
			GammaInst = 1.2;
			GammaC = 1.5;
			//PlateThicknessBoltCoeff = 0.5;
			//PlateThicknessWeldCoeff = 0.5;
			MemberLengthRatio = 2.0;
			//ShearGapElement = true;
			//ShearGapSize = 1.0;
			CheckDetailing = false;
			//StiffnessEvaluation = false;
			BracedSystem = false;
			DistanceBetweenBolts = 2.2;
			DistanceBetweenBoltsEdge = 1.2;
			DistanceDiameterBetweenBP = 4.0;

			//DeformationCapacity = true;
			ApplyConeBreakoutCheck = ConeBreakoutCheckType.Both;
			//ApplyGeneralDiagram = false;
			//const double r = 0.008;
			//double ass = Math.PI * r * r;
			//double fu = 800e6;
			//Kt = 210e9 * ass / 0.2;
			//Ks = Kt;
			//Fls = Flt = fu * ass;
			//Ktn = 1e-3;
			//Ksn = 1e-3;
			BearingCheck = false;
			BearingAngle = 26.57 * Math.PI / 180;
			DecreasingFtrd = 0.85;
			//WeldEccFactor = 0.0;
			//WeldEpsFactor = 0.08;
			GammaM3 = 1.25;
			PretensionForceFpc = 0.7;
			FrictionCoefficientPbolt = 0.3;
			//WeldRefFactor = 1.0;
			ExtensionLengthRationOpenSections = 0.5;
			ExtensionLengthRationCloseSections = 0.5;
			FactorPreloadBolt = 0.0;
			BaseMetalCapacity = true ;
			AlphaCC = 1.0;
			ApplyBearingCheck = false;
			LimitDeformation = 0.03;
			LimitDeformationCheck = false;
			AnalysisGNL = true;
			AnalysisAllGNL = false;
			//WeldingTypeSNIP = WeldingTypeSNIP.AutomaticAndMachine;
			DevelopedFillers = false;
			DeformationBoltHole = true;
			CondensedElementLengthFactor= 1.0;
		}

		/// <summary>
		/// Inicialize by setted code
		/// </summary>
		public void InitByCode()
		{
			if(SteelSetup != null)
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
		public bool ApplyBearingCheck { get; set; }

		/// <summary>
		/// Friction factor of slip-resistant joint
		/// </summary>
		public double FrictionCoefficientPbolt { get; set; }

		///// <summary>
		///// Welding types defined in SP16-Table 39
		///// </summary>
		//public WeldingTypeSNIP WeldingTypeSNIP { get; set; }

		/// <summary>
		/// Crt check type
		/// </summary>
		public CrtCompCheckIS CrtCompCheckIS { get; set; }

		/// <summary>
		/// Max value of bolt grip
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
		public double GammaMu { get; set; }

		/// <summary>
		/// Limit plastic strain for high strength steel
		/// </summary>
		public double HssLimitPlasticStrain { get; set; }

		/*
		/// <summary>
		/// True if bore holes
		/// </summary>
		public bool AddBoreHoles { get; set; }

		/// <summary>
		/// Concentration Factor kj
		/// </summary>
		public double ConcentrationFactor { get; set; }


		/// <summary>
		/// True if stiffness evaluation required
		/// </summary>
		public bool StiffnessEvaluation { get; set; }


		/// <summary>
		/// Tru if interaction tension shear is needed
		/// </summary>
		[Obsolete]
		public bool ApplyInteractionBolt { get; set; }

		/// <summary>
		/// Fillet weld ecc
		/// </summary>
		public double WeldEccFactor { get; set; }

		/// <summary>
		/// Just for testing
		/// </summary>
		public double WeldEpsFactor { get; set; }

		/// <summary>
		/// Just for mesh testing
		/// </summary>
		public double WeldRefFactor { get; set; }

		#endregion SetupProperties

		#region MeshProperties

		/// <summary>
		/// Refinement radius [borehole d]
		/// </summary>
		public double SizeRefinement { get; set; }

		/// <summary>
		/// Size of RBE3 links takes into account PlateThicknessBoltCoeff * thickness of connected plate
		/// </summary>
		public double PlateThicknessBoltCoeff { get; set; }

		/// <summary>
		/// Size of weld MPC links takes into account PlateThicknessWeldCoeff * thickness of connected plate
		/// </summary>
		public double PlateThicknessWeldCoeff { get; set; }

		/// <summary>
		/// Use FtRd as bolt tension limit
		/// </summary>
		[Obsolete]
		public bool BoltTensionLimitDecreased { get; set; }

		/// <summary>
		/// Create shear gap element in bolt link
		/// </summary>
		public bool ShearGapElement { get; set; }

		/// <summary>
		/// Alternative method to evaluate bolt stiffness C2
		/// </summary>
		[Obsolete]
		public double StiffCoeffC2 { get; set; }

		/// <summary>
		/// Borehole radius  * ShearGapSize is external size of shear gap elements
		/// </summary>
		public double ShearGapSize { get; set; }

		/// <summary>
		/// True if Buckling analysis is available
		/// </summary>
		[Obsolete]
		public bool BucklingAnalysis { get; set; }

		/// <summary>
		/// True if deformation capacity is required
		/// </summary>
		public bool DeformationCapacity { get; set; }

		#endregion MeshProperties

		#region GeneralMatProperties

		/// <summary>
		/// Just for experienced users
		/// </summary>
		public bool ApplyGeneralDiagram { get; set; }

		/// <summary>
		/// Tension linear stiffness
		/// </summary>
		public double Kt { get; set; }

		/// <summary>
		/// Limit force in tension - FLT
		/// </summary>
		public double Flt { get; set; }

		/// <summary>
		/// Nonlinear stiffness coeff in tension after over loaded FLT
		/// </summary>
		public double Ktn { get; set; }

		/// <summary>
		/// Shear linear stiffness
		/// </summary>
		public double Ks { get; set; }

		/// <summary>
		/// Limit force in shear - FLS
		/// </summary>
		public double Fls { get; set; }

		/// <summary>
		/// Nonlinear stiffness coeff in shear after over loaded FLS
		/// </summary>
		public double Ksn { get; set;
		*/
	}
}