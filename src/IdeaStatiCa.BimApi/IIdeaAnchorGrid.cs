using IdeaRS.OpenModel.Parameters;

namespace IdeaStatiCa.BimApi
{
	/// <summary>
	/// Represent anchor grid extend IIdeaFastenerGrid
	/// </summary>
	public interface IIdeaAnchorGrid : IIdeaFastenerGrid
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

		/// <summary>
		/// Anchoring Length
		/// </summary>
		double AnchoringLength
		{
			get;
			set;
		}

		/// <summary>
		/// Length of anchor hook<br/>
		/// (distance from the inner surface of the anchor shaft to the outer tip of the hook specified as an anchor diameter multiplier)
		/// </summary>
		double HookLength { get; set; }
	}
}
