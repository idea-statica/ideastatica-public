using BimApiLinkCadExample.BimApi;
using BimApiLinkCadExample.Conversion;
using CadExampleApi;
using IdeaStatiCa.BimApi;

namespace BimApiLinkCadExample.Importers
{
	/// <summary>
	/// Imports a negative plate: a body that cuts an opening into a part.
	/// </summary>
	internal class NegativePlateImporter : BaseImporter<IIdeaNegativePlate>
	{
		public NegativePlateImporter(ICadGeometryApi cad)
			: base(cad)
		{
		}

		/// <summary>
		/// Converts the negative plate with the given CAD id.
		/// </summary>
		public override IIdeaNegativePlate Create(int id)
		{
			CadPlate plate = Cad.GetPlate(id);
			CadCoordinateSystem plane = plate.Outline.CoordinateSystem;

			return new NegativePlate(id)
			{
				Name = $"NP{id}",
				MaterialName = plate.MaterialName,
				Thickness = plate.Thickness,
				OriginNodeId = NodeId.Of(plane.Origin),
				LocalCoordinateSystem = plane.ToIdea(),
				Geometry = plate.Outline.ToIdea(),
			};
		}
	}
}
