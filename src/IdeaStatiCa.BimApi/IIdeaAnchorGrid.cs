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

	public interface IIdeaConcreteBlock : IIdeaObject
	{
		/// <summary>
		/// Lenght
		/// </summary>
		double Lenght { get; }

		/// <summary>
		/// Width
		/// </summary>
		double Width { get; }

		/// <summary>
		/// Height
		/// </summary>
		double Height { get; }

		/// <summary>
		/// Material of the concrete block.
		/// </summary>
		IIdeaMaterial Material { get; }
	}
}
