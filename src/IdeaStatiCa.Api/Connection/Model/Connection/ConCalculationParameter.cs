using System.Collections.Generic;

namespace IdeaStatiCa.Api.Connection.Model
{
	/// <summary>
	/// Options for CBFEM analysis
	/// </summary>
	public class ConCalculationParameter
	{
		/// <summary>
		/// List of connections in the project to be analyzed
		/// </summary>
		public List<int> ConnectionIds { get; set; }

		/// <summary>
		/// Type of analysis to be performed
		/// </summary>
		public ConAnalysisTypeEnum AnalysisType { get; set; }
	}
}