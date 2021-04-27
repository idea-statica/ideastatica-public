using IdeaRS.OpenModel.Material;

namespace IdeaStatiCa.BimApi
{
	public interface IIdeaMaterialReinforcement : IIdeaMaterial
	{
		MatReinforcement Material { get; }
	}
}