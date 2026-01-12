using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;

namespace ConnectionIomGenerator.Fea
{
	/// <summary>
	/// Represents a Finite Element Analysis (FEA) model containing structural elements and connection points.
	/// This model serves as an intermediate representation before conversion to IDEA StatiCa Open Model (IOM).
	/// </summary>
	internal class FeaModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FeaModel"/> class.
		/// All collections are initialized as empty dictionaries.
		/// </summary>
		public FeaModel()
		{
			Materials = new Dictionary<int, IdeaMaterialByName>();
			Nodes = new Dictionary<int, IdeaNode>();
			CrossSections = new Dictionary<int, IdeaCrossSectionByName>();
			LineSegments = new Dictionary<int, IdeaLineSegment3D>();
			Elements1D = new Dictionary<int, IdeaElement1D>();
			Members1D = new Dictionary<int, IdeaMember1D>();
			ConnectionPoints = new Dictionary<int, IIdeaConnectionPoint>();
		}

		/// <summary>
		/// Gets or sets the collection of materials used in the model.
		/// Key represents the unique material identifier.
		/// </summary>
		internal Dictionary<int, IdeaMaterialByName> Materials { get; set; }

		/// <summary>
		/// Gets or sets the collection of cross-sections used in the model.
		/// Key represents the unique cross-section identifier.
		/// Cross-sections are referenced by name and must be resolved by the user in IDEA StatiCa.
		/// </summary>
		internal Dictionary<int, IdeaCrossSectionByName> CrossSections { get; set; }

		/// <summary>
		/// Gets or sets the collection of nodes (points in 3D space) in the model.
		/// Key represents the unique node identifier.
		/// Nodes serve as connection points for structural members.
		/// </summary>
		internal Dictionary<int, IdeaNode> Nodes { get; set; }

		/// <summary>
		/// Gets or sets the collection of line segments defining the geometry of elements.
		/// Key represents the unique line segment identifier.
		/// Each segment connects two nodes and defines the shape of a 1D element.
		/// </summary>
		internal Dictionary<int, IdeaLineSegment3D> LineSegments { get; set; }
		
		/// <summary>
		/// Gets or sets the collection of 1D elements in the model.
		/// Key represents the unique element identifier.
		/// Elements are parts of members and contain geometric and cross-section information.
		/// </summary>
		internal Dictionary<int, IdeaElement1D> Elements1D { get; set; }
		
		/// <summary>
		/// Gets or sets the collection of 1D members (structural elements like beams or columns) in the model.
		/// Key represents the unique member identifier.
		/// Each member consists of one or more elements and represents a complete structural component.
		/// </summary>
		internal Dictionary<int, IdeaMember1D> Members1D { get; set; }
		
		/// <summary>
		/// Gets or sets the collection of connection points in the model.
		/// Key represents the unique connection point identifier.
		/// Connection points define how members are connected together and contain information about
		/// connected members, plates, bolts, welds, and other connection components.
		/// </summary>
		internal Dictionary<int, IIdeaConnectionPoint> ConnectionPoints { get; set; }
	}
}
