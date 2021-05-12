using IdeaRS.OpenModel.CrossSection;
using IdeaRS.OpenModel.Geometry2D;

namespace IdeaStatiCa.BimApi
{
    /// <summary>
    /// A cross-section of a general cold formed profile defined by it's center line.
    /// </summary>
    public interface IIdeaCrossSectionByCenterLine : IIdeaCrossSection
    {
        /// <summary>
        /// Material of the cross-section.
        /// </summary>
        IIdeaMaterial Material { get; }

        /// <summary>
        /// Type of the cross-section.
        /// </summary>
        CrossSectionType Type { get; }

        /// <summary>
        /// Center line
        /// </summary>
        PolyLine2D CenterLine { get; }

        /// <summary>
        /// Radius
        /// </summary>
        double Radius { get; }

        /// <summary>
        /// Thickness
        /// </summary>
        double Thickness { get; }
    }
}