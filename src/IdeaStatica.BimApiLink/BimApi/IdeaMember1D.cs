using IdeaStatica.BimApiLink.Identifiers;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApi.Results;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatica.BimApiLink.BimApi
{
	public class IdeaMember1D : AbstractIdeaObject<IIdeaMember1D>, IIdeaMember1D
	{
		public virtual IIdeaPersistenceToken Token { get; set; }

		public virtual IdeaRS.OpenModel.Model.Member1DType Type { get; set; }

		public virtual List<IIdeaElement1D> Elements { get; set; } = null;

		public virtual IIdeaTaper Taper { get; set; } = null;

		public virtual IIdeaCrossSection CrossSection { get; set; } = null;

		public virtual IdeaRS.OpenModel.Model.Alignment Alignment { get; set; }

		public virtual bool MirrorY { get; set; }

		public virtual bool MirrorZ { get; set; }

		public IdeaMember1D(Identifier<IIdeaMember1D> identifer)
			: base(identifer)
		{
			Token = identifer;
		}

		public IdeaMember1D(int id)
			: this(new IntIdentifier<IIdeaMember1D>(id))
		{ }

		public IdeaMember1D(string id)
			: this(new StringIdentifier<IIdeaMember1D>(id))
		{ }

		public virtual IEnumerable<IIdeaResult> GetResults()
			=> Enumerable.Empty<IIdeaResult>();
	}
}