using IdeaRS.OpenModel.Material;

namespace IdeaStatiCa.BimApi
{
	public interface IIdeaMaterialSteel : IIdeaMaterial
	{
		MatSteel Material { get; }
	}
}