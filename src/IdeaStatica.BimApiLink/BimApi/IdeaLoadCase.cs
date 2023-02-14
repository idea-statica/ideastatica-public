using IdeaStatica.BimApiLink.Identifiers;
using IdeaStatiCa.BimApi;

namespace IdeaStatica.BimApiLink.BimApi
{
	public class IdeaLoadCase : AbstractIdeaObject<IIdeaLoadCase>, IIdeaLoadCase
	{
		public virtual IdeaRS.OpenModel.Loading.LoadCaseType LoadType { get; set; }
		
		public virtual IdeaRS.OpenModel.Loading.LoadCaseSubType Type { get; set; }
		
		public virtual IdeaRS.OpenModel.Loading.VariableType Variable { get; set; }
		
		public virtual IIdeaLoadGroup LoadGroup { get; set; } = null;
		
		public virtual string Description { get; set; } = null;
		
		public IdeaLoadCase(Identifier<IIdeaLoadCase> identifer)
			: base(identifer)
		{ }

		public IdeaLoadCase(int id)
			: this(new IntIdentifier<IIdeaLoadCase>(id))
		{ }

		public IdeaLoadCase(string id)
			: this(new StringIdentifier<IIdeaLoadCase>(id))
		{ }
	}
}
