using System.Collections.Generic;

namespace IdeaStatiCa.BimApi
{
	/// <summary>
	/// Representation of member2D
	/// </summary>
	public interface IIdeaMember2D : IIdeaObjectConnectable, IIdeaObjectWithResults
	{
		/// <summary>
		/// List of Element2D
		/// </summary>
		List<IIdeaElement2D> Elements2D { get; }

		/// <summary>
		/// Array of internal members, ribs
		/// </summary>
		List<IIdeaMember1D> Members1D { get; }
	}
}
