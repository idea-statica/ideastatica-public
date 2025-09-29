using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApi.Results;
using IdeaStatiCa.BimApiLink.Identifiers;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.BimApiLink.BimApi
{
	public class IdeaElement1D : AbstractIdeaObject<IIdeaElement1D>, IIdeaElement1D
	{
		public virtual IIdeaCrossSection StartCrossSection { get; set; } = null;

		public virtual IIdeaCrossSection EndCrossSection { get; set; } = null;

		public virtual double RotationRx { get; set; }

		public virtual IIdeaSegment3D Segment { get; set; } = null;

		public IdeaElement1D(Identifier<IIdeaElement1D> identifer)
			: base(identifer)
		{ }

		public IdeaElement1D(int id)
			: this(new IntIdentifier<IIdeaElement1D>(id))
		{ }

		public IdeaElement1D(string id)
			: this(new StringIdentifier<IIdeaElement1D>(id))
		{ }

		public virtual IEnumerable<IIdeaResult> GetResults()
			=> Enumerable.Empty<IIdeaResult>();
	}
}