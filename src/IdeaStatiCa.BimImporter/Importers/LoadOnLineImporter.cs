using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Loading;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal class LoadOnLineImporter : AbstractImporter<IIdeaLoadOnLine>
	{
		public LoadOnLineImporter(IPluginLogger logger) : base(logger)
		{
		}

		protected override OpenElementId ImportInternal(IImportContext ctx, IIdeaLoadOnLine loadOnLine)
		{
			return new LoadOnLine()
			{
				RelativeBeginPosition = loadOnLine.RelativeBeginPosition,
				RelativeEndPosition = loadOnLine.RelativeEndPosition,
				ExY = loadOnLine.ExY,
				ExZ = loadOnLine.ExZ,
				ExYEnd = loadOnLine.ExYEnd,
				ExZEnd = loadOnLine.ExZEnd,
				Type = loadOnLine.Type,
				Direction = loadOnLine.Direction,
				Bimp = loadOnLine.Bimp,
				Eimp = loadOnLine.Eimp,
				Geometry = ctx.Import(loadOnLine.Geometry),
				LoadProjection = loadOnLine.LoadProjection
			};
		}
	}
}
