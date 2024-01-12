using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Connection;
using IdeaRS.OpenModel.Geometry3D;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal class WeldImporter : AbstractImporter<IIdeaWeld>
	{
		public WeldImporter(IPluginLogger logger) : base(logger)
		{
		}

		protected override object ImportInternal(IImportContext ctx, IIdeaWeld weld, ConnectionData connectionData)
		{
			WeldData weldIOM = new WeldData()
			{
				Id = 0,
				ConnectedPartIds = weld.ConnectedParts.Select(cp => cp.Id).ToList(),
				Start = ctx.Import(weld.Start).Element as Point3D,
				End = ctx.Import(weld.End).Element as Point3D,
				Material = weld.Material?.Name,
				Thickness = weld.Thickness,
				Name = weld.Name,
				WeldType = weld.WeldType
			};

			(connectionData.Welds ?? (connectionData.Welds = new List<WeldData>())).Add(weldIOM);

			//set correct Id
			weldIOM.Id = connectionData.Welds.Max(w => w.Id) + 1;

			return weldIOM;
		}

		protected override OpenElementId ImportInternal(IImportContext ctx, IIdeaWeld obj)
		{
			throw new System.NotImplementedException();
		}
	}
}