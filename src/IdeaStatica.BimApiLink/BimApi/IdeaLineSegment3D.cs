using IdeaStatica.BimApiLink.Identifiers;
using IdeaStatiCa.BimApi;

namespace IdeaStatica.BimApiLink.BimApi
{
	public class IdeaLineSegment3D : AbstractIdeaObject<IIdeaLineSegment3D>, IIdeaLineSegment3D
	{
		public virtual IIdeaNode StartNode { get; set; } = null!;
		
		public virtual IIdeaNode EndNode { get; set; } = null!;
		
		public virtual IdeaRS.OpenModel.Geometry3D.CoordSystem LocalCoordinateSystem { get; set; } = null!;
		
		protected IdeaLineSegment3D(Identifier<IIdeaLineSegment3D> identifer)
			: base(identifer)
		{ }

		public IdeaLineSegment3D(int id)
			: this(new IntIdentifier<IIdeaLineSegment3D>(id))
		{ }

		public IdeaLineSegment3D(string id)
			: this(new StringIdentifier<IIdeaLineSegment3D>(id))
		{ }
	}
}
