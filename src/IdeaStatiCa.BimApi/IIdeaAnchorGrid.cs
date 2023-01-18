using IdeaRS.OpenModel.Parameters;

namespace IdeaStatiCa.BimApi
{
	public interface IIdeaAnchorGrid : IIdeaBoltGrid
	{
		/// <summary>
		/// Anchor Type
		/// </summary>
		AnchorType AnchorType { get; }

		/// <summary>
		/// Concrete Block
		/// </summary>
		IIdeaConcreteBlock ConcreteBlock { get; }

		/// <summary>
		/// Washer Size
		/// </summary>
		double WasherSize { get; }
	}
}
