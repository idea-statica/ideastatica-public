using IdeaRS.OpenModel.Geometry2D;
using IdeaRS.OpenModel.Geometry3D;
using IdeaStatiCa.BimApiLink.Utils;
using BimApiLinkCadExample.BimApi;
using IdeaStatiCa.BimApi;
using System;
using System.Linq;
using System.Windows;
using BimApiLinkCadExample.CadExampleApi;
using IdeaStatiCa.BimApiLink.BimApi;

namespace BimApiLinkCadExample.Importers
{
	internal class PlateImporter : BaseImporter<IIdeaPlate>
	{
		public PlateImporter(ICadGeometryApi model): base(model)
		{
		}

		public override IIdeaPlate Create(int id)
		{
			var item = Model.GetPlate(id);
			
			if (item != null)
			{
				var p = new Plate(item.Id);
				if (FillPlateInstance(item, p))
				{
					return p;
				}
				else
				{
					return null;
				}
			}

			return null;
		}

		private bool FillPlateInstance(CadPlate cadplate, Plate plate) 
		{
			plate.MaterialNo = Model.GetMaterialByName(cadplate.Material).Id;

			CadPlane3D plane = cadplate.CadOutline2D.Plane;

			var localCoordinateSystem = plane.ToIdea();
			
			plate.LocalCoordinateSystem = localCoordinateSystem;

			plate.Thickness = cadplate.Thickness;
			plate.Geometry = cadplate.CadOutline2D.ToIdea();
			plate.OriginNo = Utils.PointTranslator.GetPointId(cadplate.CadOutline2D.Plane.Origin);

			return true;
		}
	}
}
