using IdeaRS.OpenModel.Message;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace IdeaRS.OpenModel.Concrete.CheckResult
{
	/// <summary>
	/// The number in enum value description means the priority for extreme check evaluation.
	/// </summary>
	public enum CheckResult
	{
		/// <summary>
		/// 3. check value is bigger than limit check value
		/// </summary>
		Failed = 0,

		/// <summary>
		/// 5. check value is lesser than limit check value
		/// </summary>
		Passed,

		/// <summary>
		/// 4. check value is lesser than limit check value, but there is a warning in calculation
		/// </summary>
		PassedWithWarnings,

		/// <summary>
		/// 2. error appeared during calculation, calculation was terminated
		/// </summary>
		FailedWithError,

		/// <summary>
		/// 7. this kind of the calculation was switched off
		/// </summary>
		CheckIsOff,

		/// <summary>
		/// 1. this kind of the calculation was not done
		/// </summary>
		NotDone,

		/// <summary>
		/// 6. check for some reason not necessary
		/// </summary>
		CheckIsNotNecesssary,
	}

	/// <summary>
	/// Calculation type
	/// </summary>
	public enum CalculationType
	{
		/// <summary>
		/// not defined check
		/// </summary>
		NotDefined,

		/// <summary>
		/// calculation of capacity by the interaction diagram
		/// </summary>
		Capacity,

		/// <summary>
		/// calculation plane of deformation
		/// </summary>
		Response,

		/// <summary>
		/// shear check
		/// </summary>
		Shear,

		/// <summary>
		/// torsion check
		/// </summary>
		Torsion,

		/// <summary>
		/// interaction shear+torsion
		/// </summary>
		Interaction,

		/// <summary>
		/// calculation - response for fatigue
		/// </summary>
		Fatigue,

		/// <summary>
		/// stress limitation check - short
		/// </summary>
		StressLimitationShort,

		/// <summary>
		/// stress limitation check - long
		/// </summary>
		StressLimitationLong,

		/// <summary>
		/// stress limitation check - stage
		/// </summary>
		StressLimitationStage,

		/// <summary>
		/// crack width check - short
		/// </summary>
		CrackWidthShort,

		/// <summary>
		/// crack width check - long
		/// </summary>
		CrackWidthLong,

		/// <summary>
		/// crack width check - stage
		/// </summary>
		CrackWidthStage,

		/// <summary>
		/// stiffness calculation - short
		/// </summary>
		StiffnessShort,

		/// <summary>
		/// stiffness calculation - long
		/// </summary>
		StiffnessLong,

		/// <summary>
		/// stiffness calculation - stage
		/// </summary>
		StiffnessStage,

		/// <summary>
		/// deflection check
		/// </summary>
		Deflection,

		/// <summary>
		/// brittle failure check
		/// </summary>
		BrittleFailure,

		/// <summary>
		/// detailing check
		/// </summary>
		Detailing,

		/// <summary>
		/// Linear stress check - short
		/// </summary>
		LinearStressShort,

		/// <summary>
		/// Linear stress check - long
		/// </summary>
		LinearStressLong,

		/// <summary>
		/// Linear stress check - stage
		/// </summary>
		LinearStressStage,

		/// <summary>
		/// calculation cross-section characteristics - short
		/// </summary>
		CrossSectionCharacteristicsShort,

		/// <summary>
		/// calculation cross-section characteristics - long
		/// </summary>
		CrossSectionCharacteristicsLong,

		/// <summary>
		/// calculation cross-section characteristics - stage
		/// </summary>
		CrossSectionCharacteristicsStage,

		/// <summary>
		/// calculation internal forces for slabs/shells/walls
		/// </summary>
		Baumann,

		/// <summary>
		/// all internal forces are recalculated because of slenderness/imperfections etc...
		/// </summary>
		OverallColumnRecalculation,

		/// <summary>
		/// Recalculation of internal forces on compression members (effect of geometric imperfections and second order effect).
		/// </summary>
		ColumnForces,

		/// <summary>
		/// calculation prestress effects
		/// </summary>
		PrestressEffects,

		/// <summary>
		/// calculation primary forces
		/// </summary>
		Primaryforces,

		/// <summary>
		/// calculation long term losses calculation
		/// </summary>
		LongTermLosses,

		/// <summary>
		/// calculation M-N-Kappa diagram ULS
		/// </summary>
		MNKappaDiagramULS,

		/// <summary>
		/// calculation M-N-Kappa diagram SLS-short
		/// </summary>
		MNKappaDiagramSLSShort,

		/// <summary>
		/// calculation M-N-Kappa diagram SLS-long
		/// </summary>
		MNKappaDiagramSLSLong,

		/// <summary>
		/// calculation of creep and shrinkage
		/// </summary>
		CreepAndShrinkageCoefficient,

		/// <summary>
		/// calculation of creep and shrinkage
		/// </summary>
		ConcreteCover,

		/// <summary>
		/// calculation if cross section is cracked
		/// </summary>
		CrackingCalculation,

		/// <summary>
		/// calculation if cross section is cracked
		/// </summary>
		CrackingCalculationShort,

		/// <summary>
		/// calculation if cross section is cracked
		/// </summary>
		CrackingCalculationLong,

		/// <summary>
		/// calculation of redistribution
		/// </summary>
		Redistribution,

		/// <summary>
		/// initial state of cross-section
		/// </summary>
		InitilaStateofCrossSection,

		/// <summary>
		/// angle of concrete strut
		/// </summary>
		StrutAngle,

		/// <summary>
		/// desing of reinforcement
		/// </summary>
		DesignReinforcement,

		/// <summary>
		/// interaction diagram on mesh
		/// </summary>
		CapacityMesh,

		/// <summary>
		/// calculation siffness with effective modulus of elasticity
		/// </summary>
		StiffnessStageLong,

		/// <summary>
		/// calculation long-term losses coefficient
		/// </summary>
		LongTermLossesCoefficient
	}

	/// <summary>
	/// Concrete result base class
	/// </summary>
	[XmlInclude(typeof(ConcreteSLSCheckResultCrackWidthEc2))]
	[XmlInclude(typeof(ConcreteSLSCheckResultStressLimitationEc2))]
	[XmlInclude(typeof(ConcreteULSCheckResultDiagramCapacityEc2))]
	[XmlInclude(typeof(ConcreteULSCheckResultInteractionEc2))]
	[XmlInclude(typeof(ConcreteULSCheckResultResponseEc2))]
	[XmlInclude(typeof(ConcreteULSCheckResultShearEc2))]
	[XmlInclude(typeof(ConcreteULSCheckResultTorsionEc2))]
	[XmlInclude(typeof(ConcreteCheckDetailingEc2))]
	[XmlInclude(typeof(ConcreteULSCheckResultFatigueEc2))]
	[XmlInclude(typeof(ConcreteSLSCheckResultStiffnessEc2))]
	public abstract class ConcreteCheckResultBase
	{
		/// <summary>
		/// Constructor
		/// </summary>
		protected ConcreteCheckResultBase()
		{
			NonConformities = new List<NonConformity>();
		}

		/// <summary>
		/// internal forces used for calculation
		/// </summary>
		public IdeaRS.OpenModel.Result.ResultOfInternalForces InternalFores { get; set; }

		/// <summary>
		/// Returns nonconformity in section
		/// </summary>
		public List<NonConformity> NonConformities { get; set; }

		/// <summary>
		/// check result passed/failed
		/// </summary>
		public CheckResult Result { get; set; }

		/// <summary>
		/// calculated limited value, calculated as strain to limit strain
		/// </summary>
		public double CheckValue { get; set; }

		/// <summary>
		/// limit check value for result check
		/// </summary>
		public double LimitCheckValue { get; set; }

		/// <summary>
		/// settings of check type
		/// </summary>
		public CalculationType Check { get; set; }
	}
}