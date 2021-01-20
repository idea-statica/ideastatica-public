using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.Connection
{
	/// <summary>
	/// Defines a report type.
	/// </summary>
	public enum ReportType
	{
		/// <summary>
		/// Brief report - only summary is created.
		/// </summary>
		Brief,

		/// <summary>
		/// Detailed report - with all results related to report settings.
		/// </summary>
		Detailed,
	}

	/// <summary>
	/// Type formulas output
	/// </summary>
	public enum FormulasType
	{
		/// <summary>
		/// Formulas are not presented in report
		/// </summary>
		Disabled,

		/// <summary>
		/// Formulas are presented for extreme result
		/// </summary>
		Extreme,

		/// <summary>
		/// Formulas are presented for all results
		/// </summary>
		All
	}

	/// <summary>
	/// Defines a unit type.
	/// </summary>
	public enum UnitType
	{
		/// <summary>
		/// Metric
		/// </summary>
		Metric,

		/// <summary>
		/// Imperial
		/// </summary>
		Imperial,
	}

	/// <summary>
	/// Report settings for IDEA connections.
	/// </summary>
	[DataContract]
	public class ReportSettings
	{
		/// <summary>
		/// Gets or sets the type of report.
		/// </summary>
		[DataMember]
		public ReportType ReportType { get; set; }

		/// <summary>
		/// Indicates, whether generate views (XY, XZ, YZ) into report.
		/// </summary>
		[DataMember]
		public bool Views { get; set; }

		/// <summary>
		/// Indicates, whether generate pictures of results.
		/// </summary>
		[DataMember]
		public bool ResultsPictures { get; set; }

		/// <summary>
		/// Indicates, whether generate explanations to report.
		/// </summary>
		[DataMember]
		public bool SymbolExplanations { get; set; }

		/// <summary>
		/// Indicates, whether generate drawing of plates.
		/// </summary>
		[DataMember]
		public bool PlatesDrawings { get; set; }

		/// <summary>
		/// Indicates, whether generate theoretical background.
		/// </summary>
		[DataMember]
		public bool TheoreticalBackground { get; set; }

		/// <summary>
		/// The name of the required culture of the report
		/// </summary>
		[DataMember]
		public string CultureName { get; set; }

		/// <summary>
		/// The name of the required culture of the report
		/// </summary>
		[DataMember]
		public UnitType Unit { get; set; }

		/// <summary>
		/// Indicates if formulas are in report
		/// </summary>
		[DataMember]
		public FormulasType Formulas { get; set; }
	}
}
