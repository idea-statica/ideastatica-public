using IdeaRS.OpenModel.CrossSection;
using IdeaRS.OpenModel.Geometry2D;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;

namespace IdeaStatiCa.BimApiLink.BimApi
{
	public class IdeaCrossSectionByCenterLine : AbstractIdeaObject<IIdeaCrossSectionByCenterLine>, IIdeaCrossSectionByCenterLine
	{
		public IdeaCrossSectionByCenterLine(Identifier<IIdeaCrossSectionByCenterLine> identifier)
			: base(identifier)
		{ }

		public IdeaCrossSectionByCenterLine(int id)
			: this(new IntIdentifier<IIdeaCrossSectionByCenterLine>(id))
		{ }

		public IdeaCrossSectionByCenterLine(string id)
			: this(new StringIdentifier<IIdeaCrossSectionByCenterLine>(id))
		{ }

		public virtual IIdeaMaterial Material { get; set; } = null;

		public CrossSectionType Type { get; set; }

		public PolyLine2D CenterLine { get; set; } = null;

		public double Radius { get; set; }

		public double Thickness { get; set; }

		public double Rotation { get; set; }

		public bool IsInPrincipal { get; set; }
	}
}