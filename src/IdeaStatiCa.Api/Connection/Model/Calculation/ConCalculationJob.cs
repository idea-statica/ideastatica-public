using System;
using System.Collections.Generic;

namespace IdeaStatiCa.Api.Connection.Model
{
	/// <summary>
	/// State of an asynchronous CBFEM calculation job started by the calculate-async endpoints.
	/// At most one job can be active per project; poll it by <see cref="JobId"/> and cancel it
	/// with the DELETE endpoint.
	/// </summary>
	public class ConCalculationJob
	{
		/// <summary>Identifier used to poll and cancel the job.</summary>
		public Guid JobId { get; set; }

		/// <summary>Id of the project the job calculates in.</summary>
		public Guid ProjectId { get; set; }

		public ConCalculationJobStatusEnum Status { get; set; }

		/// <summary>Connections the job calculates, in execution order.</summary>
		public List<int> ConnectionIds { get; set; }

		/// <summary>Number of connections whose calculation has finished (successfully or not).</summary>
		public int ConnectionsCompleted { get; set; }

		/// <summary>Id of the connection currently being calculated; null when not running.</summary>
		public int? CurrentConnectionId { get; set; }

		/// <summary>
		/// Progress of the current connection's solve as a fraction (0-1), stepping once per solved
		/// load case - the value the desktop progress bar consumes. Coarse by design: it reports which
		/// load case is being solved, not progress within it.
		/// </summary>
		public double Progress { get; set; }

		/// <summary>Last progress message from the solver (current load case and iteration), if any.</summary>
		public string Message { get; set; }

		public DateTime CreatedAt { get; set; }

		public DateTime? FinishedAt { get; set; }

		/// <summary>Result summaries of the calculated connections; populated when the job finishes.</summary>
		public List<ConResultSummary> Results { get; set; }

		/// <summary>Failure reason; populated when the job status is <see cref="ConCalculationJobStatusEnum.Failed"/>.</summary>
		public string Error { get; set; }
	}
}
