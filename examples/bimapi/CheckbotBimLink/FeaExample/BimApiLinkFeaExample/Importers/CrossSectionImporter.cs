using BimApiLinkFeaExample.BimApi;
using FeaExampleApi;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Importers;

namespace BimApiLinkFeaExample.Importers
{
	/// <summary>
	/// Imports a cross-section by its FEA id.
	/// </summary>
	internal class CrossSectionImporter : IntIdentifierImporter<IIdeaCrossSection>
	{
		private readonly IFeaGeometryApi _geometry;

		public CrossSectionImporter(IFeaGeometryApi geometry)
		{
			_geometry = geometry;
		}

		/// <summary>
		/// The cross-section with the given FEA id. Checkbot looks its name up in the IDEA StatiCa library.
		/// </summary>
		public override IIdeaCrossSection Create(int id)
		{
			FeaCrossSection crossSection = _geometry.GetCrossSection(id);

			return new CrossSectionByName(id)
			{
				Name = crossSection.Name,
				MaterialId = crossSection.MaterialId,
			};
		}
	}
}
