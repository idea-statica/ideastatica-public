using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;
using System.Collections.Generic;

namespace IdeaStatiCa.BimApiLink.BimApi
{
	public class IdeaPolyLine3D : AbstractIdeaObject<IIdeaPolyLine3D>, IIdeaPolyLine3D
	{
		public virtual List<IIdeaSegment3D> Segments { get; set; }

		public IdeaPolyLine3D(Identifier<IIdeaPolyLine3D> identifer)
			: base(identifer)
		{ }

		public IdeaPolyLine3D(int id)
			: this(new IntIdentifier<IIdeaPolyLine3D>(id))
		{ }

		public IdeaPolyLine3D(string id)
			: this(new StringIdentifier<IIdeaPolyLine3D>(id))
		{ }
	}
}
