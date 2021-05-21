using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Loading;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal class LoadGroupImporter : AbstractImporter<IIdeaLoadGroup>
	{
		public LoadGroupImporter(IPluginLogger logger) : base(logger)
		{
		}

		protected override OpenElementId ImportInternal(IImportContext ctx, IIdeaLoadGroup lg)
		{
			LoadGroupEC lgRet = new LoadGroupEC()
			{
				Name = lg.Name,
				Dzeta = lg.Dzeta,
				GammaGInf = lg.GammaGInf,
				GammaGSup = lg.GammaGSup,
				GammaQ = lg.GammaQ,
				GroupType = lg.GroupType,
				Psi0 = lg.Psi0,
				Psi1 = lg.Psi1,
				Psi2 = lg.Psi2,
				Relation = lg.Relation

			};

			return lgRet;
		}
	}
}
