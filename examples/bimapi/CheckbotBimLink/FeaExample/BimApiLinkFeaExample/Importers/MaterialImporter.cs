using FeaExampleApi;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;
using IdeaStatiCa.BimApiLink.Importers;

namespace BimApiLinkFeaExample.Importers
{
	/// <summary>
	/// Imports a material by its FEA id.
	/// </summary>
	internal class MaterialImporter : IntIdentifierImporter<IIdeaMaterial>
	{
		private readonly IFeaGeometryApi _geometry;

		public MaterialImporter(IFeaGeometryApi geometry)
		{
			_geometry = geometry;
		}

		/// <summary>
		/// The steel of the given FEA material. Checkbot looks its name up among the IDEA StatiCa library steels.
		/// </summary>
		public override IIdeaMaterial Create(int id)
		{
			FeaMaterial material = _geometry.GetMaterial(id);

			return new IdeaMaterialByName(id)
			{
				Name = material.Name,
				MaterialType = MaterialType.Steel,
			};
		}
	}
}
