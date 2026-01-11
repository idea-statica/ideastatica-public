using IdeaStatiCa.BimApiLink.BimApi;

namespace ConnectionIomGenerator.Fea
{
	internal class FeaModel
	{
		public FeaModel()
		{
			Materials = new Dictionary<int, IdeaMaterialByName>();
			Nodes = new Dictionary<int, IdeaNode>();
			CrossSections = new Dictionary<int, IdeaCrossSectionByName>();
			LineSegments = new Dictionary<int, IdeaLineSegment3D>();
			Elements1D = new Dictionary<int, IdeaElement1D>();
			Members1D = new Dictionary<int, IdeaMember1D>();
		}

		internal Dictionary<int, IdeaMaterialByName> Materials { get; set; }

		internal Dictionary<int, IdeaCrossSectionByName> CrossSections { get; set; }

		internal Dictionary<int, IdeaNode> Nodes { get; set; }

		internal Dictionary<int, IdeaLineSegment3D> LineSegments { get; set; }
		internal Dictionary<int, IdeaElement1D> Elements1D { get; set; }
		internal Dictionary<int, IdeaMember1D> Members1D { get; set; }
	}
}
