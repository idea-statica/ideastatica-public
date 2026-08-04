namespace IdeaStatiCa.Api.Connection.Model
{
	/// <summary>
	/// Lifecycle state of an asynchronous CBFEM calculation job.
	/// </summary>
	public enum ConCalculationJobStatusEnum
	{
		/// <summary>The job was accepted and waits for the project to become available.</summary>
		Queued,

		/// <summary>The job is calculating.</summary>
		Running,

		/// <summary>All requested connections were calculated; <c>Results</c> holds their summaries.</summary>
		Finished,

		/// <summary>The job ended with an error; <c>Error</c> holds the reason.</summary>
		Failed,

		/// <summary>The job was cancelled; connections whose solve was interrupted stay not-calculated.</summary>
		Cancelled
	}
}
