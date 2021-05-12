using IdeaRS.OpenModel.Material;

namespace IdeaStatiCa.BimApi
{
    /// <summary>
    ///
    /// </summary>
    public interface IIdeaMaterialReinforcement : IIdeaMaterial
    {
        MatReinforcement Material { get; }
    }
}