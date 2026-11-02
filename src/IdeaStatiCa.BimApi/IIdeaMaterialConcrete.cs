using IdeaRS.OpenModel.Material;

namespace IdeaStatiCa.BimApi
{
	/// <summary>
	/// A concrete material.
	/// </summary>
	public interface IIdeaMaterialConcrete : IIdeaMaterial
	{
		/// <summary>
		/// Parameters of the material.
		/// </summary>
		MatConcrete Material { get; }
	}
}