using IdeaRS.OpenModel.Material;

namespace IdeaStatiCa.BimApi
{
	public interface IIdeaMaterialConcrete : IIdeaMaterial
	{
		MatConcrete Material { get; }
	}
}