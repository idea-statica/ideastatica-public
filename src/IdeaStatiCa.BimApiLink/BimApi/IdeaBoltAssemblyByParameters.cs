using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;

namespace IdeaStatiCa.BimApiLink.BimApi
{
	public class IdeaBoltAssemblyByParameters : AbstractIdeaObject<IIdeaBoltAssemblyByParameters>, IIdeaBoltAssemblyByParameters
	{
		protected IdeaBoltAssemblyByParameters(Identifier<IIdeaBoltAssemblyByParameters> identifier) : base(identifier)
		{ }

		public IdeaBoltAssemblyByParameters(string id) : this(new StringIdentifier<IIdeaBoltAssemblyByParameters>(id))
		{ }

		public double HoleDiameter { get; set; }

		public double Diameter { get; set; }

		public double HeadDiameter { get; set; }

		public double DiagonalHeadDiameter { get; set; }

		public double HeadHeight { get; set; }

		public double BoreHole { get; set; }

		public double TensileStressArea { get; set; }

		public double NutThickness { get; set; }

		public string Standard { get; set; }

		public double GrossArea { get; set; }

		public double WasherThickness { get; set; }
		public bool WasherAtHead { get; set; }
		public bool WasherAtNut { get; set; }

		public virtual IIdeaMaterial BoltGrade { get; set; } = null;
	}
}
