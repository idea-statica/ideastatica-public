using BimApiLinkCadExample.BimApi;
using CadExampleApi;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Importers;

namespace BimApiLinkCadExample.Importers
{
	/// <summary>
	/// Imports a cross-section by its name.
	/// </summary>
	internal class CrossSectionImporter : StringIdentifierImporter<IIdeaCrossSection>
	{
		private readonly ICadGeometryApi _cad;

		public CrossSectionImporter(ICadGeometryApi cad)
		{
			_cad = cad;
		}

		/// <summary>
		/// The cross-section with the given name. Checkbot looks the name up in the IDEA StatiCa library.
		/// </summary>
		public override IIdeaCrossSection Create(string name)
		{
			CadCrossSection crossSection = _cad.GetCrossSection(name);

			return new CrossSectionByName(name)
			{
				Name = crossSection.Name,
				MaterialName = crossSection.MaterialName,
			};
		}
	}
}
