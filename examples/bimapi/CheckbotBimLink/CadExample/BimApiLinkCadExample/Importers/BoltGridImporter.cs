using BimApiLinkCadExample.BimApi;
using BimApiLinkCadExample.Conversion;
using CadExampleApi;
using IdeaRS.OpenModel.Parameters;
using IdeaStatiCa.BimApi;
using System;
using System.Linq;

namespace BimApiLinkCadExample.Importers
{
	/// <summary>
	/// Imports a bolt grid with its bolt assembly.
	/// </summary>
	internal class BoltGridImporter : BaseImporter<IIdeaBoltGrid>
	{
		public BoltGridImporter(ICadGeometryApi cad)
			: base(cad)
		{
		}

		/// <summary>
		/// Converts the bolt grid with the given CAD id. Each bolt position becomes a node.
		/// </summary>
		public override IIdeaBoltGrid Create(int id)
		{
			CadBoltGrid boltGrid = Cad.GetBoltGrid(id);
			CadCoordinateSystem plane = boltGrid.CoordinateSystem;

			return new BoltGrid(id)
			{
				OriginNodeId = NodeId.Of(plane.Origin),
				LocalCoordinateSystem = plane.ToIdea(),
				Positions = boltGrid.Positions.Select(position => Get<IIdeaNode>(NodeId.Of(plane.ToWorld(position)))).ToList(),
				ConnectedParts = boltGrid.ConnectedParts.Select(GetConnectedPart).ToList(),
				BoltAssembly = CreateBoltAssembly(boltGrid.BoltAssembly),
				BoltShearType = BoltShearType.Interaction,
				ShearInThread = false,
			};
		}

		/// <summary>
		/// The IDEA StatiCa object for a part the bolts hold: a member or a plate.
		/// </summary>
		private IIdeaObjectConnectable GetConnectedPart(ICadPart part)
		{
			switch (part)
			{
				case CadMember member:
					return Get<IIdeaMember1D>(member.Id);
				case CadPlate plate:
					return Get<IIdeaPlate>(plate.Id);
				default:
					throw new NotSupportedException($"Bolts cannot hold a {part.GetType().Name}.");
			}
		}

		/// <summary>
		/// The bolt type, with its dimensions from the CAD library. Its grade is imported by name when BimApiLink reads
		/// it.
		/// </summary>
		private static BoltAssembly CreateBoltAssembly(CadBoltAssembly assembly)
		{
			return new BoltAssembly(assembly.Name)
			{
				Name = assembly.Name,
				BoltGradeName = assembly.GradeName,
				Diameter = assembly.Diameter,
				BoreHole = assembly.HoleDiameter,
				HeadDiameter = assembly.HeadDiameter,
				DiagonalHeadDiameter = assembly.HeadDiagonalDiameter,
				HeadHeight = assembly.HeadHeight,
				NutThickness = assembly.NutThickness,
				GrossArea = assembly.GrossArea,
				TensileStressArea = assembly.TensileStressArea,
			};
		}
	}
}
