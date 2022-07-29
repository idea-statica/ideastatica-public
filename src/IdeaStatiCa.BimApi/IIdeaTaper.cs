using System.Collections.Generic;

namespace IdeaStatiCa.BimApi
{
	/// <summary>
	/// Defines haunches (variyng cross-sections) along the member.
	///
	/// One <see cref="IIdeaTaper"/> may be assigned to multiple <see cref="IIdeaMember1D">Members</see>.
	/// Sections of the member not covered by a span will use the member's cross-section.
	/// </summary>
	public interface IIdeaTaper : IIdeaObject
	{
		/// <summary>
		/// List of <see cref="IIdeaSpan">spans</see>. Spans mut not overlap.
		/// </summary>
		IEnumerable<IIdeaSpan> Spans { get; }
	}
}