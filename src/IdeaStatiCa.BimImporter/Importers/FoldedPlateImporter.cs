using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Connection;
using IdeaRS.OpenModel.Geometry3D;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.Extensions;
using IdeaStatiCa.Plugin;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal class FoldedPlateImporter : AbstractImporter<IIdeaFoldedPlate>
	{
		public FoldedPlateImporter(IPluginLogger logger) : base(logger)
		{
		}

		protected override object ImportInternal(IImportContext ctx, IIdeaFoldedPlate foldedPlate, ConnectionData connectionData)
		{

			var foldedPlateIOM = new FoldedPlateData()
			{
				Plates = foldedPlate.Plates.Select(p =>
				{
					return ctx.ImportConnectionItem(p, connectionData) as PlateData;
				}).ToList(),

				Bends = foldedPlate.Bends.Select(fp => new BendData()
				{
					EndFaceNormal1 = fp.EndFaceNormal.ToIOMVector(),
					Point1OfSideBoundary1 = ctx.Import(fp.LineOnSideBoundary1.StartNode).Element as Point3D,
					Point2OfSideBoundary1 = ctx.Import(fp.LineOnSideBoundary1.EndNode).Element as Point3D,
					Point1OfSideBoundary2 = ctx.Import(fp.LineOnSideBoundary2.StartNode).Element as Point3D,
					Point2OfSideBoundary2 = ctx.Import(fp.LineOnSideBoundary2.EndNode).Element as Point3D,
					Radius = fp.Radius,
					Plate1Id = (ctx.ImportConnectionItem(fp.Plate1, connectionData) as PlateData)?.Id ?? 0,
					Plate2Id = (ctx.ImportConnectionItem(fp.Plate2, connectionData) as PlateData)?.Id ?? 0,
				}).ToList(),
			};


			(connectionData.FoldedPlates ?? (connectionData.FoldedPlates = new List<FoldedPlateData>())).Add(foldedPlateIOM);

			foldedPlateIOM.Plates.ForEach(plate => connectionData.Plates.Remove(plate));

			return foldedPlateIOM;
		}

		protected override OpenElementId ImportInternal(IImportContext ctx, IIdeaFoldedPlate obj)
		{
			throw new System.NotImplementedException();
		}
	}
}