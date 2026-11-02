using System.Collections.Generic;

namespace IdeaRS.OpenModel.Loading
{
	/// <summary>
	/// Load case type
	/// </summary>
	public enum LoadCaseType
	{
		/// <summary>
		/// Permanent
		/// </summary>
		Permanent = 0,

		/// <summary>
		/// Variable
		/// </summary>
		Variable = 1,

		/// <summary>
		/// Accidental
		/// </summary>
		Accidental = 2,

		///// <summary>
		///// Additional load case (Hidden for UI)
		///// </summary>
		//Hidden,

		///// <summary>
		///// Additional load case - moving (Hidden for UI)
		///// </summary>
		//HiddenMoving = 4,

		/// <summary>
		/// Loadcase is for nonlinear analysis
		/// </summary>
		Nonlinear = 5,
	}

	/// <summary>
	/// Load case sub type
	/// </summary>
	public enum LoadCaseSubType
	{
		/// <summary>
		/// Permanent - self weight
		/// </summary>
		PermanentSelfweight = 0,

		/// <summary>
		/// Permanent - standard
		/// </summary>
		PermanentStandard = 1,

		/// <summary>
		/// Permanent - prestress (posttensioned)
		/// </summary>
		PermanentPrestress = 2,

		/// <summary>
		/// Permanent - Rheologic
		/// </summary>
		PermanentRheologic = 3,

		/// <summary>
		/// Permanent - primary effect
		/// </summary>
		PermanentPrimaryEffect = 7,

		/// <summary>
		/// Variable - static
		/// </summary>
		VariableStatic = 9,

		/// <summary>
		/// Variable - dynamic
		/// </summary>
		VariableDynamic = 10,

		/// <summary>
		/// Variable - primary effect
		/// </summary>
		VariablePrimaryEffect = 11,

		/// <summary>
		/// Permanent - prestress (posttensioned)
		/// </summary>
		PermanentPrestressPretensioned = 15,

		/// <summary>
		///  Permanent - prestress primary effect
		/// </summary>
		PermanentPrestressPrimary = 16,

		/// <summary>
		/// Permanent - self weight - local
		/// </summary>
		PermanentSelfweightLocal = 17,
	}

	/// <summary>
	/// Variable type
	/// </summary>
	public enum VariableType
	{
		/// <summary>
		/// Standard
		/// </summary>
		Standard,

		/// <summary>
		/// Mobile envelope
		/// </summary>
		MobileEnvelope,

		/// <summary>
		/// Seismicity
		/// </summary>
		Seismicity,

		/// <summary>
		/// variable loads is not set for some reasons - in fact loads is not variable
		/// </summary>
		NotSet,
	}

	/// <summary>
	/// Duration of load case
	/// </summary>
	public enum TypeDuration
	{
		/// <summary>
		/// Long term
		/// </summary>
		LongTerm = 0,

		/// <summary>
		/// Medium term
		/// </summary>
		MediumTerm = 1,

		/// <summary>
		/// Short term
		/// </summary>
		ShortTerm = 2,

		/// <summary>
		/// Instantaneous term
		/// </summary>
		InstantaneousTerm = 3,
	}

	/// <summary>
	/// Load case in the model
	/// </summary>
	/// <example> 
	/// This sample shows how to create a load case.
	/// <code lang = "C#">
	/// //Creating the model
	/// OpenModel openModel = new OpenModel();
	/// 
	/// //Load group - needed for Load case
	/// LoadGroupEC loadGroup = new LoadGroupEC();
	/// loadGroup.Name = "LG1";
	/// loadGroup.GammaQ = 1.5;
	/// loadGroup.Psi0 = 0.7;
	/// loadGroup.Psi1 = 0.5;
	/// loadGroup.Psi2 = 0.3;
	/// loadGroup.GammaGInf = 1.0;
	/// loadGroup.GammaGSup = 1.35;
	/// loadGroup.Dzeta = 0.85;
	/// openModel.AddObject(loadGroup);
	/// 
	/// //Load case
	/// LoadCase loadCase = new LoadCase();
	/// loadCase.Name = "LC1";
	/// loadCase.LoadType = LoadCaseType.Permanent;
	/// loadCase.Type = LoadCaseSubType.PermanentStandard;
	/// loadCase.Variable = VariableType.NotSet;
	/// loadCase.LoadGroup = new ReferenceElement(loadGroup);
	/// openModel.AddObject(loadCase);
	/// </code>
	/// </example>
	[OpenModelClass("CI.StructModel.Loading.LoadCase,CI.Loading", "CI.StructModel.Loading.ILoadCase,CI.BasicTypes")]
	public class LoadCase : OpenElementId
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public LoadCase()
		{
			LoadsInPoint = new List<ReferenceElement>();
			LoadsOnLine = new List<ReferenceElement>();
			StrainLoadsOnLine = new List<ReferenceElement>();
			PointLoadsOnLine = new List<ReferenceElement>();
			LoadsOnSurface = new List<ReferenceElement>();
			Settlements = new List<ReferenceElement>();
			TemperatureLoadsOnLine = new List<ReferenceElement>();
		}

		/// <summary>
		/// Name of load case
		/// </summary>
		public System.String Name { get; set; }

		/// <summary>
		/// Load case type
		/// </summary>
		public LoadCaseType LoadType { get; set; }

		/// <summary>
		/// Sub type
		/// </summary>
		public LoadCaseSubType Type { get; set; }

		/// <summary>
		/// Variable type
		/// </summary>
		public VariableType Variable { get; set; }

		///// <summary>
		///// Duration - this variable is never used , it should be removed
		///// </summary>
		//public TypeDuration Duration { get; set; }

		/// <summary>
		/// Load group
		/// </summary>
		public ReferenceElement LoadGroup { get; set; }

		/// <summary>
		/// Additional info
		/// </summary>
		public System.String Description { get; set; }

		/// <summary>
		/// List of point load impulses in this load case
		/// </summary>
		public System.Collections.Generic.List<ReferenceElement> LoadsInPoint { get; set; }

		/// <summary>
		/// List of line load impulses in this load case
		/// </summary>
		public System.Collections.Generic.List<ReferenceElement> LoadsOnLine { get; set; }

		/// <summary>
		/// List of generalized strain load impulses along the line in this load case.
		/// </summary>
		public System.Collections.Generic.List<ReferenceElement> StrainLoadsOnLine { get; set; }

		/// <summary>
		/// List of point load impulses in this load case
		/// </summary>
		public System.Collections.Generic.List<ReferenceElement> PointLoadsOnLine { get; set; }

		/// <summary>
		/// List surafce load in this load case
		/// </summary>
		public System.Collections.Generic.List<ReferenceElement> LoadsOnSurface { get; set; }

		/// <summary>
		/// Settlements in this load case
		/// </summary>
		public System.Collections.Generic.List<ReferenceElement> Settlements { get; set; }

		/// <summary>
		/// List of temperature load in this load case
		/// </summary>
		public System.Collections.Generic.List<ReferenceElement> TemperatureLoadsOnLine { get; set; }
	}
}