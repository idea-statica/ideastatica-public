using IdeaRS.OpenModel.CrossSection;
using IdeaRS.OpenModel.Geometry2D;

namespace IdeaStatiCa.BimApi
{
    public interface IIdeaCrossSectionByCenterLine : IIdeaCrossSection
    {
        IIdeaMaterial Material { get; }

        CrossSectionType Type { get; }

        PolyLine2D CenterLine { get; }

        double Radius { get; }

        double Thickness { get; }
    }
}