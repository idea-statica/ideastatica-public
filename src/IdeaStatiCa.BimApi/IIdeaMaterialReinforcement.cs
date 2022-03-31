using IdeaRS.OpenModel.Material;

namespace IdeaStatiCa.BimApi
{
	/// <summary>
	/// Material of a reinforcement.
	/// </summary>
	public interface IIdeaMaterialReinforcement : IIdeaMaterial
	{
		/// <summary>
		/// Material
		/// </summary>
		MatReinforcement Material { get; }
	}
}