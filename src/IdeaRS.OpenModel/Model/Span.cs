namespace IdeaRS.OpenModel.Model
{
	/// <summary>
	/// Span allows specifying haunched member.
	/// 
	/// Both cross-section must be of the same type. Spans must not overlap.
	/// </summary>
	[OpenModelClass("CI.StructModel.Structure.Span,CI.StructuralElements", "CI.StructModel.Structure.ISpan,CI.BasicTypes")]
	public class Span : OpenElementId
	{
		/// <summary>
		/// <see cref="CrossSection.CrossSection">Cross-section</see> at the start position of the span. Must not be null.
		/// </summary>
		public ReferenceElement StartCrossSection { get; set; }

		/// <summary>
		/// <see cref="CrossSection.CrossSection">Cross-section</see> at the end position of the span.
		/// </summary>
		public ReferenceElement EndCrossSection { get; set; }

		/// <summary>
		/// Relative position on the member where the span starts.
		/// </summary>
		public double StartPosition { get; set; }

		/// <summary>
		/// Relative position on the member where the span ends.
		/// </summary>
		public double EndPosition { get; set; }
	}
}