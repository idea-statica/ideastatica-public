using IdeaRS.OpenModel.Geometry2D;

namespace IdeaStatiCa.BimApi
{
    /// <summary>
    /// A component of a <see cref="IIdeaCrossSectionByComponents"/>.
    /// </summary>
    public interface IIdeaCrossSectionComponent
    {
        /// <summary>
        /// Material of the component.
        /// </summary>
        IIdeaMaterial Material { get; }

        /// <summary>
        /// Geometry of the component.
        /// </summary>
        Region2D Geometry { get; }

        /// <summary>
        /// Phase
        /// </summary>
        int Phase { get; }
    }
}