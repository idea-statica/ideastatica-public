using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Connection;
using IdeaRS.OpenModel.Geometry3D;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.Extensions;
using IdeaStatiCa.Plugin;
using System.Collections.Generic;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal class CutImporter : AbstractImporter<IIdeaCut>
	{
		public CutImporter(IPluginLogger logger) : base(logger)
		{
		}

		protected override object ImportInternal(IImportContext ctx, IIdeaCut cut, ConnectionData connectionData)
		{

			if (cut.CuttingObject is IIdeaWorkPlane workPlane)
			{
				var cutData = new CutData()
				{
					NormalVector = workPlane.Normal.ToIOMVector(),
					Offset = cut.Offset,
					Direction = cut.CutOrientation,
					PlanePoint = ctx.Import(workPlane.Origin).Element as Point3D,

				};
				var beamIOM = connectionData.Beams.Find(b => b.OriginalModelId == cut.ModifiedObject.Id);
				if (beamIOM != null)
				{
					(beamIOM.Cuts ?? (beamIOM.Cuts = new List<CutData>())).Add(cutData);
				}

				return cutData;
			}
			else
			{
				var beamIOM = connectionData.Beams.Find(b => b.OriginalModelId == cut.ModifiedObject.Id);
				if (beamIOM != null)
				{
					var cutIOM = new IdeaRS.OpenModel.Connection.CutBeamByBeamData
					{
						CuttingObject = new ReferenceElement(ctx.ImportConnectionItem(cut.CuttingObject, connectionData) as OpenElementId),
						ModifiedObject = new ReferenceElement(beamIOM),
						IsWeld = cut.Weld != null,
						Method = cut.CutMethod,
						Orientation = cut.CutOrientation,
						PlaneOnCuttingObject = cut.DistanceComparison,
						WeldThickness = cut.Weld != null ? cut.Weld.Thickness : 0.0,
						WeldType = cut.Weld != null ? cut.Weld.WeldType : WeldType.Fillet,
					};

					(connectionData.CutBeamByBeams ?? (connectionData.CutBeamByBeams = new List<CutBeamByBeamData>())).Add(cutIOM);

					return cutIOM;
				}

				return null;
			}
		}

		protected override OpenElementId ImportInternal(IImportContext ctx, IIdeaCut cut)
		{
			throw new System.NotImplementedException();
		}

	}
}
