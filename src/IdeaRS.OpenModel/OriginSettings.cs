using System;

namespace IdeaRS.OpenModel
{

	/// <summary>
	/// CountryCode
	/// </summary>
	public enum CountryCode
	{
		/// <summary>
		/// no code
		/// </summary>
		None = 0,

		/// <summary>
		/// Eurocode
		/// </summary>
		ECEN = 1,

		/// <summary>
		/// India code
		/// </summary>
		India = 5,

		/// <summary>
		/// Swiss code
		/// </summary>
		SIA = 6,

		/// <summary>
		/// American design code as ACI, AISC, ASTM standards etc.
		/// </summary>
		American = 8,

		/// <summary>
		/// Canadian design code
		/// </summary>
		Canada = 9,

		/// <summary>
		/// Australian design code
		/// </summary>
		Australia = 10,

		/// <summary>
		/// Russia design code
		/// </summary>
		RUS = 11,

		/// <summary>
		/// China design code
		/// </summary>
		CHN = 12,

		/// <summary>
		/// Hong Kong design code
		/// </summary>
		HKG = 13,
	}

	/// <summary>
	/// Conversion table of cross-section
	/// </summary>
	public enum CrossSectionConversionTable
	{
		/// <summary>
		/// Conversion table will be not used
		/// </summary>
		NoUsed = 0,

		/// <summary>
		/// Conversion table of SCIA CSS will be used
		/// </summary>
		SCIA = 1,

		/// <summary>
		/// Conversion table of Autodesk CSS will be used, it is useable for Robot, Revit, ...
		/// </summary>
		Autodesk = 2,

		/// <summary>
		/// Conversion table of SAP2000 will be used, it is useable for SAP2000 ...
		/// </summary>
		SAP2000 = 3,

		/// <summary>
		/// Conversion table of StaadPro CSS will be used, it is useable for StaadPro, ...
		/// </summary>
		StaadPro = 4,

		/// <summary>
		/// Conversion table of AdvanceDesign CSS will be used, it is useable for AdvanceDesign, ...
		/// </summary>
		AdvanceDesign = 5
	}

	/// <summary>
	/// OriginProject
	/// </summary>
	public class OriginSettings : OpenObject
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public OriginSettings()
		{
			CrossSectionConversionTable = IdeaRS.OpenModel.CrossSectionConversionTable.NoUsed;
			CountryCode = CountryCode.ECEN;
		}
		/// <summary>
		/// Name of project
		/// </summary>
		public string ProjectName { get; set; }

		/// <summary>
		/// Description of project
		/// </summary>
		public string ProjectDescription { get; set; }

		/// <summary>
		/// Name of author of project
		/// </summary>
		public string Author { get; set; }

		/// <summary>
		/// Date of create project 
		/// </summary>
		public DateTime DateOfCreate { get; set; }

		/// <summary>
		/// Conversion table for Parameters
		/// </summary>
		public CrossSectionConversionTable CrossSectionConversionTable { get; set; }

		/// <summary>
		///  CountryCode
		/// </summary>
		public CountryCode CountryCode { get; set; }

		/// <summary>
		///  Import also recommended welds
		/// </summary>
		public bool ImportRecommendedWelds { get; set; }

		/// <summary>
		///  All loads have to be in equilibrium
		/// </summary>
		public bool CheckEquilibrium { get; set; }
	}
}
