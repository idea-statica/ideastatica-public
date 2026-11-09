using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;

namespace BimApiLinkCadExample.BimApi
{
	/// <summary>
	/// A bolt assembly whose grade is imported when BimApiLink reads it.
	/// </summary>
	internal class BoltAssembly : IdeaBoltAssemblyByParameters
	{
		public BoltAssembly(string name)
			: base(name)
		{
		}

		public string BoltGradeName { get; set; }

		public override IIdeaMaterial BoltGrade => Get<IIdeaMaterial>(BoltGradeName);
	}
}
