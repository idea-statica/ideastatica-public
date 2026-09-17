using System;
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
		/// Not used. The service reads the analysis type from the connection itself
		/// (<see cref="ConConnection.AnalysisType"/>), not from the calculation request.
		/// Set the analysis type with a connection update before calculating.
		/// </summary>
		[Obsolete("Not used - the service ignores this value. The analysis type is read from the connection itself (ConConnection.AnalysisType); set it with a connection update before calculating.")]
		public ConAnalysisTypeEnum AnalysisType { get; set; }
	}
}