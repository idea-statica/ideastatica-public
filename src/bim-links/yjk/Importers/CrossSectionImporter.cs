using yjk.BimApis;
using IdeaStatiCa.BimApiLink.Importers;
using IdeaStatiCa.BimApi;

namespace yjk.Importers
{
	internal class CrossSectionImporter : IntIdentifierImporter<IIdeaCrossSection>
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