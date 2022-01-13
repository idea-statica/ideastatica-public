using IdeaStatiCa.BimApi;
using IdeaStatiCa.RamToIdea.BimApi;
using IdeaStatiCa.RamToIdea.Geometry;
using IdeaStatiCa.RamToIdea.Sections;
using RAMDATAACCESSLib;

namespace IdeaStatiCa.RamToIdea.Factories
{
	internal class ObjectFactory : IObjectFactory
	{
		private readonly IModel _model;
		private readonly ISectionFactory _sectionFactory;
		private readonly IGeometry _geometry;
		private readonly ISegmentFactory _segmentFactory;

		public ObjectFactory(IModel model, ISectionFactory sectionFactory, IGeometry geometry, ISegmentFactory segmentFactory)
		{
			_model = model;
			_sectionFactory = sectionFactory;
			_geometry = geometry;
			_segmentFactory = segmentFactory;
		}

		public IIdeaMember1D GetBeam(IBeam beam)
		{
			return new RamMemberBeam(this, _sectionFactory, _geometry, _segmentFactory, beam);
		}

		public IIdeaMember1D GetColumn(IColumn column)
		{
			return new RamMemberColumn(this, _sectionFactory, _geometry, _segmentFactory, column);
		}

		public IIdeaMember1D GetHorizontalBrace(IHorizBrace horizBrace)
		{
			return new RamMemberHorizontalBrace(this, _sectionFactory, _geometry, _segmentFactory, horizBrace);
		}

		public IIdeaMember1D GetVerticalBrace(IVerticalBrace verticalBrace)
		{
			return new RamMemberVerticalBrace(this, _sectionFactory, _geometry, _segmentFactory, verticalBrace);
		}

		public IStory GetStory(int uid)
		{
			return _model.GetStory(uid);
		}
	}
}