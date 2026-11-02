using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;

namespace IdeaStatiCa.BimApiLink.BimApi
{
	public class IdeaBoltAssemblyByName : AbstractIdeaObject<IIdeaBoltAssemblyByName>, IIdeaBoltAssemblyByName
	{

		public IdeaBoltAssemblyByName(Identifier<IIdeaBoltAssemblyByName> identifer)
			: base(identifer)
		{ }

		public IdeaBoltAssemblyByName(int id)
			: this(new IntIdentifier<IIdeaBoltAssemblyByName>(id))
		{ }

		public IdeaBoltAssemblyByName(string id)
			: this(new StringIdentifier<IIdeaBoltAssemblyByName>(id))
		{ }
	}
}
