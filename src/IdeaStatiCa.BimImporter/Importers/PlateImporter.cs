using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Connection;
using IdeaRS.OpenModel.Geometry3D;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal class PlateImporter : AbstractImporter<IIdeaPlate>
	{
		public PlateImporter(IPluginLogger logger) : base(logger)
		{
		}

		protected override object ImportInternal(IImportContext ctx, IIdeaPlate plate, ConnectionData connectionData)
		{
			var lcs = plate.LocalCoordinateSystem as IdeaRS.OpenModel.Geometry3D.CoordSystemByVector;

			PlateData plateIOM = new PlateData()
			{
				Geometry = plate.Geometry,
				//Id = plate.Id,
				AxisX = lcs.VecX,
				AxisY = lcs.VecY,
				AxisZ = lcs.VecZ,
				Material = plate.Material.Name,
				Name = plate.Name,
				OriginalModelId = plate.Token.ToString(),
				Origin = ctx.Import(plate.Origin).Element as Point3D,
				Thickness = plate.Thickness,
				IsNegativeObject = plate is IIdeaNegativePlate,
			};

			(connectionData.Plates ?? (connectionData.Plates = new List<PlateData>())).Add(plateIOM);

			//set correct Id
			plateIOM.Id = connectionData.Plates.Max(p => p.Id) + 1;

			return plateIOM;
		}

		protected override OpenElementId ImportInternal(IImportContext ctx, IIdeaPlate obj)
		{
			throw new System.NotImplementedException();
		}
	}
}