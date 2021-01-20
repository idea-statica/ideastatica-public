namespace IdeaRS.OpenModel.Concrete
{
	/// <summary>
	/// Concrete calculation setup
	/// </summary>
	public class CalculationSetup : OpenObject
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public CalculationSetup()
		{
			UlsDiagram = true;
			UlsResponse = false;
			UlsShear = true;
			UlsTorsion = true;
			UlsInteraction = true;
			SlsCrack = true;
			SlsStressLimitation = true;
			Detailing = true;
			MNKappaDiagram = false;
			Fatigue = true;
			CrossSectionCharacteristics = false;
			SlsStiffnesses = false;
			SlsDeflections = false;
		}

		/// <summary>
		/// ULS response
		/// </summary>
		public bool UlsResponse { get; set; }

		/// <summary>
		/// ULS capacity N-M-M
		/// </summary>
		public bool UlsDiagram { get; set; }

		/// <summary>
		/// ULS shear
		/// </summary>
		public bool UlsShear { get; set; }

		/// <summary>
		/// ULS torsion
		/// </summary>
		public bool UlsTorsion { get; set; }

		/// <summary>
		/// Uls interaction
		/// </summary>
		public bool UlsInteraction { get; set; }

		/// <summary>
		/// SLS crack
		/// </summary>
		public bool SlsCrack { get; set; }

		/// <summary>
		/// SLS stress limitation
		/// </summary>
		public bool SlsStressLimitation { get; set; }

		/// <summary>
		/// Detailing
		/// </summary>
		public bool Detailing { get; set; }

		/// <summary>
		/// M-N-κ diagram
		/// </summary>
		public bool MNKappaDiagram { get; set; }

		/// <summary>
		/// ULS fatigue
		/// </summary>
		public bool Fatigue { get; set; }

		/// <summary>
		/// Cross section charactertistics
		/// </summary>
		public bool CrossSectionCharacteristics { get; set; }

		/// <summary>
		/// SLS stiffnesses
		/// </summary>
		public bool SlsStiffnesses { get; set; }

		/// <summary>
		/// SLS deflections
		/// </summary>
		public bool SlsDeflections { get; set; }
		
		//public bool SlsBrittleFailure { get; set; }
		//public bool UlsLateralShear { get; set; }
		//public bool ConcreteCover { get; set; }
		//public bool CreepAndShrinkage { get; set; }
		//public bool LinearStress { get; set; }
		//public bool Redistribution { get; set; }
		//public bool StateOfCrossSection { get; set; }
		//public bool InitialStateOfCrossSection { get; set; }
		//public bool DesignReinforcement { get; set; }
	}
}