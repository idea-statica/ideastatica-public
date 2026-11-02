namespace IdeaStatiCa.BimApi
{
	/// <summary>
	/// Span allows specifying haunched member.
	///
	/// Both cross-sect
	/// </summary>
	public interface IIdeaSpan : IIdeaObject
	{
		/// <summary>
		/// <see cref="IIdeaCrossSection">Cross-section</see> at the start position of the span. Must not be null.
		/// </summary>
		IIdeaCrossSection StartCrossSection { get; }

		/// <summary>
		/// <see cref="IIdeaCrossSection">Cross-section</see> at the end position of the span.
		/// </summary>
		IIdeaCrossSection EndCrossSection { get; }

		/// <summary>
		/// Relative position on the member where the span starts.
		/// </summary>
		double StartPosition { get; }

		/// <summary>
		/// Relative position on the member where the span ends.
		/// </summary>
		double EndPosition { get; }
	}
}