using IdeaRS.OpenModel.Geometry3D;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;
using System.Collections.Generic;

namespace IdeaStatiCa.BimApiLink.BimApi
{
	public class IdeaRegion3D : AbstractIdeaObject<IIdeaRegion3D>, IIdeaRegion3D
	{
		public virtual IIdeaPolyLine3D Outline { get; set; }

		public virtual List<IIdeaPolyLine3D> Openings { get; set; }

		public virtual CoordSystem LocalCoordinateSystem { get; set; }

		public IdeaRegion3D(Identifier<IIdeaRegion3D> identifer)
			: base(identifer)
		{ }

		public IdeaRegion3D(int id)
			: this(new IntIdentifier<IIdeaRegion3D>(id))
		{ }

		public IdeaRegion3D(string id)
			: this(new StringIdentifier<IIdeaRegion3D>(id))
		{ }
	}
}
