using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaRS.OpenModel.Loading
{

	/// <summary>
	/// ResultClass object
	/// </summary>
	[OpenModelClass("CI.StructModel.Loading.ResultClass,CI.Loading", "CI.StructModel.Loading.IResultClass,CI.BasicTypes", typeof(ResultClass))]
	public class ResultClass : OpenElementId
	{

		/// <summary>
		/// constructor
		/// </summary>
		public ResultClass()
		{
			ListItem = new List<SelectionItemForResults>();
		}
		
		/// <summary>
		/// Name of ResultClass
		/// </summary>
		public string Name{ get; set; }

		/// <summary>
		/// List of objects in this library
		/// </summary>
		public List<SelectionItemForResults> ListItem { get; set; }

		/// <summary>
		/// Description of IResultClass 
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// TypeResultsClass
		/// </summary>
		public TypeResultsClass TypeResultClass { get; set; }

		/// <summary>
		/// Id of IConstructionStage, 0 == IConstructionStage is not defined
		/// </summary>
		public int IdConstructionStage { get; set; }

		/// <summary>
		/// Type of update RC. If TypeUpdateResultClass is Update, ListItem will be updated
		/// </summary>
		public TypeUpdateResultClass TypeUpdateResultClass { get; set; }

		/// <summary>
		/// Class contains nonlinear combinations (loaded as loadcase)
		/// </summary>
		public bool Nonlinear { get; set; }
	}


	/// <summary>
	/// TypeUpdateResultClass
	/// </summary>
	public enum TypeUpdateResultClass
	{
		/// <summary>
		/// Update
		/// </summary>
		Update,
		/// <summary>
		/// Do not update
		/// </summary>
		DoNotUpdate
	}

	/// <summary>
	/// TypeResultsClass
	/// </summary>
	public enum TypeResultsClass
	{
		/// <summary>
		/// ULSResultsClass
		/// </summary>
		ULSResultsClass,

		/// <summary>
		/// SLSResultsClass
		/// </summary>
		SLSResultsClass,

		/// <summary>
		/// ULSCheckClass
		/// </summary>
		ULSCheckClass,

		/// <summary>
		/// SLSFrequentCheckClass
		/// </summary>
		SLSFrequentCheckClass,

		/// <summary>
		/// SLSQuasiPermanentCheckClass
		/// </summary>
		SLSQuasiPermanentCheckClass,

		/// <summary>
		/// SLSCharacteristicsCheckClass
		/// </summary>
		SLSCharacteristicsCheckClass,

		/// <summary>
		/// SLSDeflectionResultsClass
		/// </summary>
		SLSDeflectionResultsClass,

		/// <summary>
		/// ULSAccidentalResultsClass
		/// </summary>
		ULSAccidentalResultsClass,

		/// <summary>
		/// FatigueResultsClass
		/// </summary>
		FatigueResultsClass,

		/// <summary>
		/// CheckClassFireResistance
		/// </summary>
		CheckClassFireResistance,

		/// <summary>
		/// CheckClassBridgeLoadRating
		/// </summary>
		CheckClassBridgeLoadRating,

		//CheckStressClass,

	}

	/// <summary>
	/// SelectionItemForResults
	/// </summary>
	[OpenModelClass("CI.StructModel.Loading.SelectionItemForResults,CI.Loading", "CI.StructModel.Loading.ISelectionItemForResults,CI.BasicTypes", typeof(SelectionItemForResults))]
	public class SelectionItemForResults : OpenElementId
	{
		/// <summary>
		/// Description
		/// </summary>
		public string Description
		{
			get;
		}

		/// <summary>
		/// TypeOfLoad
		/// </summary>
		public string TypeOfLoad
		{
			get;
		}

		/// <summary>
		/// TypeClassEvaluation
		/// </summary>
		public TypeClassEvaluation TypeClassEvaluation
		{
			get;
			set;
		}

		/// <summary>
		/// ILibraryItem
		/// </summary>
		public ReferenceElement Item
		{
			get;
			set;
		}

		/// <summary>
		/// Coefficient
		/// </summary>
		public double Coeff
		{
			get;
		}


		//		void Init(ILibraryItem iLoadCase, double p, bool generateDescription = true);
	}

	/// <summary>
	/// Load of evaluation
	/// </summary>
	public enum TypeClassEvaluation
	{
		/// <summary>
		/// LoadCase
		/// </summary>
		eLoadCase,

		/// <summary>
		/// LinCombi
		/// </summary>
		eLinCombi,

		/// <summary>
		/// NonLinCombi
		/// </summary>
		eNonLinCombi,

		///// <summary>
		///// Class
		///// </summary>
		//eClass,

		///// <summary>
		///// ResCombi
		///// </summary>
		//eResCombi,

		///// <summary>
		///// AllLoadCases
		///// </summary>
		//eAllLoadCases,
	}



}
