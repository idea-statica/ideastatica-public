using System.Collections.Generic;

namespace IdeaStatiCa.BimApi
{
	/// <summary>
	/// A generic cross-sectíons defined by its components.
	/// </summary>
	public interface IIdeaCrossSectionByComponents : IIdeaCrossSection
	{
		/// <summary>
		/// Set of components.
		/// </summary>
		HashSet<IIdeaCrossSectionComponent> Components { get; }
	}
}