using IdeaStatica.BimApiLink.Identifiers;
using IdeaStatiCa.BimApi;

namespace IdeaStatica.BimApiLink.BimApi
{
	public class IdeaBoltAssembly : AbstractIdeaObject<IIdeaBoltAssembly>, IIdeaBoltAssembly
	{
		protected IdeaBoltAssembly(Identifier<IIdeaBoltAssembly> identifier) : base(identifier)
		{ }

		public IdeaBoltAssembly(string id) : this(new StringIdentifier<IIdeaBoltAssembly>(id))
		{ }

		public double HoleDiameter { get; set; }

		public double Diameter { get; set; }

		public double HeadDiameter { get; set; }

		public double Lenght { get; set; }

		public double DiagonalHeadDiameter { get; set; }

		public double HeadHeight { get; set; }

		public double BoreHole { get; set; }

		public double TensileStressArea { get; set; }

		public double NutThickness { get; set; }

		public string Standard { get; set; }

		public virtual IIdeaMaterial Material { get; set; }

	}
}
