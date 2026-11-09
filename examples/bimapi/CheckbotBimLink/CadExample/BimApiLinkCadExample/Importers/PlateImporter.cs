using BimApiLinkCadExample.BimApi;
using BimApiLinkCadExample.Conversion;
using CadExampleApi;
using IdeaStatiCa.BimApi;

namespace BimApiLinkCadExample.Importers
{
	/// <summary>
	/// Imports a plate.
	/// </summary>
	internal class PlateImporter : BaseImporter<IIdeaPlate>
	{
		public PlateImporter(ICadGeometryApi cad)
			: base(cad)
		{
		}

		/// <summary>
		/// Converts the plate with the given CAD id. Its outline lies in the XY plane of its coordinate system, whose
		/// origin is the plate's origin node.
		/// </summary>
		public override IIdeaPlate Create(int id)
		{
			CadPlate plate = Cad.GetPlate(id);
			CadCoordinateSystem plane = plate.Outline.CoordinateSystem;

			return new Plate(id)
			{
				Name = $"P{id}",
				MaterialName = plate.MaterialName,
				Thickness = plate.Thickness,
				OriginNodeId = NodeId.Of(plane.Origin),
				LocalCoordinateSystem = plane.ToIdea(),
				Geometry = plate.Outline.ToIdea(),
			};
		}
	}
}
