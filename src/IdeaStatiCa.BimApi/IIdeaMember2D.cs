using System.Collections.Generic;

namespace IdeaStatiCa.BimApi
{
	/// <summary>
	/// Representation of member2D
	/// </summary>
	public interface IIdeaMember2D : IIdeaObjectConnectable
	{
		/// <summary>
		/// List of Element2D
		/// </summary>
		List<IIdeaElement2D> Elements2D { get; }
	}
}
