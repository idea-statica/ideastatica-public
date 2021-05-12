using System.Collections.Generic;

namespace IdeaStatiCa.BimApi
{
    /// <summary>
    /// A generic cross-sect�ons specified by its components.
    /// </summary>
    public interface IIdeaCrossSectionByComponents : IIdeaCrossSection
    {
        HashSet<IIdeaCrossSectionComponent> Components { get; }
    }
}