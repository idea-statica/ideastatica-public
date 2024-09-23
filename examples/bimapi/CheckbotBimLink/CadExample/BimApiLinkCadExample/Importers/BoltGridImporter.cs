using BimApiLinkCadExample.BimApi;
using BimApiLinkCadExample.CadExampleApi;
using BimApiLinkCadExample.Utils;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;
using System.Collections.Generic;

namespace BimApiLinkCadExample.Importers
{
	internal class BoltGridImporter : BaseImporter<IIdeaBoltGrid>
	{
		public BoltGridImporter(ICadGeometryApi model) : base(model)
		{
		}

		public override IIdeaBoltGrid Create(int id)
		{
			var item = Model.GetBoltGrid(id);

			BoltGrid boltGrid = new BoltGrid(id);
			var localCoordinateSystem = item.BoltPlane.ToIdea();

			boltGrid.LocalCoordinateSystem = localCoordinateSystem;

			boltGrid.OriginNo = PointTranslator.GetPointId(item.BoltPlane.Origin);

			var nodes = new List<IIdeaNode>();

			foreach (var point in item.BoltPositions)
			{
				var ptString = PointTranslator.GetPointId(CadPoint2D.Get2DPointInWorldCoords(point, item.BoltPlane));
				var node = Get<IIdeaNode>(ptString);
				nodes.Add(node);
			}

			boltGrid.Positions = nodes;

			boltGrid.Length = item.BoltLength;
			// create bolt assembly
			var ba = new BoltAssembly(item.BoltAssembly)
			{
				Diameter = 0.016,
				HeadDiameter = 0.024,
				DiagonalHeadDiameter = 0.026,
				HeadHeight = 0.01,
				BoreHole = 0.018,
				TensileStressArea = 157,
				NutThickness = 0.013,
				BoltGradeNo = Model.GetMaterialByName(item.BoltGrade).Id,
				Standard = item.BoltAssembly
			};

			// Get connected parts
			var cp = new List<IIdeaObjectConnectable>();

			foreach (var obj in item.ConnectedParts)
			{
				if (obj is CadPlate plate)
				{
					cp.Add(Get<IIdeaPlate>(new IntIdentifier<IIdeaPlate>(plate.PartId)));
				}
				else if (obj is CadMember member)
				{
					cp.Add(Get<IIdeaMember1D>(new IntIdentifier<IIdeaMember1D>(member.PartId)));
				}
			}

			boltGrid.ConnectedParts = cp;
			boltGrid.ShearInThread = false;
			boltGrid.BoltShearType = IdeaRS.OpenModel.Parameters.BoltShearType.Interaction;
			boltGrid.BoltAssembly = ba;

			return boltGrid;
		}
	}
}
