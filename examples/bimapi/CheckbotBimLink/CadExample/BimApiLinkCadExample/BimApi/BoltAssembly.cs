using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;


namespace BimApiLinkCadExample.BimApi
{
	internal class BoltAssembly : IdeaBoltAssemblyByParameters
	{
		public BoltAssembly(Identifier<IIdeaBoltAssemblyByParameters> identifier) : base(identifier)
		{ }

		public BoltAssembly(string id) : base(id)
		{ }

		public override IIdeaMaterial BoltGrade => Get<IIdeaMaterial>(BoltGradeNo);

		public int BoltGradeNo { get; set; }
	}
}
