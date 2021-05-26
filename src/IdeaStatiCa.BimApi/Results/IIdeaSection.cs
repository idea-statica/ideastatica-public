using System.Collections.Generic;

namespace IdeaStatiCa.BimApi.Results
{
	/// <summary>
	/// Section of <see cref="IIdeaMember1D"/> or <see cref="IIdeaElement1D"/> with results.
	/// </summary>
	public interface IIdeaSection
	{
		/// <summary>
		/// Position on <see cref="IIdeaMember1D"/>/<see cref="IIdeaElement1D"/>.
		/// Value must be within 0 and 1 (including) where 0 is the start and 1 is the end.
		/// </summary>
		double Position { get; }

		/// <summary>
		/// Results on the section.
		/// </summary>
		IEnumerable<IIdeaSectionResult> Results { get; }
	}
}