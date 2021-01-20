using IdeaRS.OpenModel.Geometry2D;

namespace IdeaRS.OpenModel.Concrete.CheckResult
{
	/// <summary>
	/// Section stiffnesses
	/// </summary>
	public class SectionStiffnesses
	{
		/// <summary>
		/// rotation around x-axis
		/// </summary>
		public double Alfa { get; set; }

		/// <summary>
		/// axial stiffness
		/// </summary>
		public double EAx { get; set; }

		/// <summary>
		/// eccentricity in x-direction
		/// </summary>
		public double EccX { get; set; }

		/// <summary>
		/// eccentricity in y-direction
		/// </summary>
		public double EccY { get; set; }

		/// <summary>
		/// eccentricity in y-direction
		/// </summary>
		public double EccZ { get; set; }

		/// <summary>
		/// bending stiffness
		/// </summary>
		public double EIy { get; set; }

		/// <summary>
		/// bending stiffness
		/// </summary>
		public double EIz { get; set; }

		/// <summary>
		/// product moment stiffness
		/// </summary>
		public double EDyz { get; set; }

		/// <summary>
		/// shear stiffness
		/// </summary>
		public double GAy { get; set; }

		/// <summary>
		/// shear stiffness
		/// </summary>
		public double GAz { get; set; }

		/// <summary>
		/// torsion stiffness
		/// </summary>
		public double GIx { get; set; }
	}

	/// <summary>
	/// SLS Check tiffness base
	/// </summary>
	public abstract class ConcreteSLSCheckResultStiffness : ConcreteCheckResultBase
	{
		/// <summary>
		/// linear stiffnesses
		/// </summary>
		public SectionStiffnesses LinearStiffness { get; set; }

		/// <summary>
		/// result stiffnesses
		/// </summary>
		public SectionStiffnesses Stiffness { get; set; }

		/// <summary>
		/// stiffnesses calculated on uncracked cross-section
		/// </summary>
		public SectionStiffnesses StiffnessI { get; set; }

		/// <summary>
		/// stiffnesses calculated on uncracked cross-section
		/// </summary>
		public SectionCharacteristics CharacteristicsI { get; set; }

		/// <summary>
		/// stiffnesses calculated on fully-cracked cross-section
		/// </summary>
		public SectionStiffnesses StiffnessII { get; set; }

		/// <summary>
		/// stiffnesses calculated on cracked cross-section
		/// </summary>
		public SectionCharacteristics CharacteristicsII { get; set; }

		///// <summary>
		///// result shrinkage state 
		///// </summary>
		//[XmlIgnore]
		//public ShrinkageState Shrinkage { get; set; }

		///// <summary>
		///// shrinkage state I- uncracked cross-section
		///// </summary>
		//[XmlIgnore]
		//public ShrinkageState ShrinkageI { get; set; }

		///// <summary>
		///// shrinkage state II- cracked cross-section
		///// </summary>
		//[XmlIgnore]
		//public ShrinkageState ShrinkageII { get; set; }

		///// <summary>
		///// total area of nonpressed reinforcement in cross-section
		///// </summary>
		//public double As { get; set; }

		///// <summary>
		///// total area of nonpressed compression reinforcement in cross-section
		///// </summary>
		//public double Asc { get; set; }

		///// <summary>
		///// total area of nonpressed tension reinforcement in cross-section
		///// </summary>
		//public double Ast { get; set; }

		///// <summary>
		///// is the stress in the tension reinforcement calculated on the basis of a
		///// cracked section
		///// </summary>
		//public double Sigma_ss { get; set; }

		///// <summary>
		///// is the stress in the tension reinforcement calculated on the basis of a
		///// cracked section under the loading conditions causing first cracking
		///// </summary>
		//public double Sigma_sr { get; set; }

		///// <summary>
		///// is a distribution coefficient (allowing for tensioning stiffening at a section)
		///// </summary>
		//public double Zeta { get; set; }

		///// <summary>
		///// is a beta coefficient
		///// </summary>
		//public double Beta { get; set; }

		///// <summary>
		///// result object contains all inputs and result necessary for calculation creep coefficient
		///// </summary>
		//public IResCreep CreepCoefficient { get; set; }

		///// <summary>
		///// result object contains all inputs and result necessary for calculation shrinkage coefficient
		///// poměrné přetvoření od volného smršťování (viz 3.1.4);
		///// </summary>
		//public IResShrinkage ShrinkageCoefficient { get; set; }

		///// <summary>
		///// plane of deformation calculated on uncracked cross-section
		///// </summary>
		//public PlaneDeformation PlaneI { get; set; }

		///// <summary>
		///// plane of deformation calculated on fully cracked cross-section
		///// </summary>
		//public PlaneDeformation PlaneII { get; set; }

		/// <summary>
		/// crack resistance internal forces
		/// </summary>
		public IdeaRS.OpenModel.Result.ResultOfInternalForces Fr { get; set; }
	}
}