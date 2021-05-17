using IdeaRS.OpenModel.Material;

namespace IdeaStatiCa.BimApi
{
	/// <summary>
	/// A steal material.
	/// </summary>
	public interface IIdeaMaterialSteel : IIdeaMaterial
	{
		/// <summary>
		/// Parameters of the material.
		/// </summary>
		MatSteel Material { get; }
	}
}