using IdeaRS.OpenModel.Parameters;

namespace IdeaStatiCa.BimApi
{
	/// <summary>
	/// Represent anchor grid extend IIdeaGrid
	/// </summary>
	public interface IIdeaAnchorGrid : IIdeaGrid
	{

		/// <summary>
		/// Shear in thread
		/// </summary>
		bool ShearInThread { get; }

		/// <summary>
		/// Defines a transfer of shear force in bolts.
		/// </summary>
		BoltShearType BoltShearType { get; }

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

		/// <summary>
		/// Bolt Assembly
		/// </summary>
		IIdeaBoltAssembly BoltAssembly { get; }
	}
}
