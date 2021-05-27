namespace IdeaStatiCa.BimApi.Results
{
	/// <summary>
	/// Describes a result in a section.
	/// </summary>
	public interface IIdeaSectionResult
	{
		/// <summary>
		/// Loading source of the result.
		/// </summary>
		IIdeaLoading Loading { get; }

		/// <summary>
		/// Data for the result.
		/// </summary>
		IIdeaResultData Data { get; }
	}
}