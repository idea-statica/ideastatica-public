using BimApiFeaLink.BimApi;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Importers;

namespace BimApiFeaLink.Importers
{
	public class CrossSectionImporter : IntIdentifierImporter<IIdeaCrossSection>
	{
		public CrossSectionImporter(/*TODO pass API using DI*/)
		{
		}

		public override IIdeaCrossSection Create(int id)
		{
			return new CrossSectionByName(id)
			{
				MaterialNo = 1,
				Name = "IPE200",
			};
		}
	}
}