using IdeaRS.OpenModel.Parameters;

namespace IdeaStatiCa.BimApi
{
	/// <summary>
	/// Represent bolt grid
	/// </summary>
	public interface IIdeaBoltGrid : IIdeaFastenerGrid
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
		/// Bolt Assembly
		/// </summary>
		IIdeaBoltAssembly BoltAssembly { get; }
	}
}