using IdeaStatiCa.BimApiLink.BimApi;
using IdeaStatiCa.BimApiLink.Importers;
using IdeaStatiCa.BimApi;
using BimApiLinkCadExample.CadExampleApi;

namespace BimApiLinkCadExample.Importers
{
	internal class MaterialImporter : BaseImporter<IIdeaMaterial>
	{
		public MaterialImporter(ICadGeometryApi model)
			: base(model)
		{
		}

		public override IIdeaMaterial Create(int id)
		{
			var material = Model.GetMaterial(id);
			
			string name = material.Name;

			if (material == null)
			{
				name = string.IsNullOrWhiteSpace(name) ? "unknown material" : name;
				
				// e.g. the bolt materail 10.9
				return new IdeaMaterialByName(id)
				{
					MaterialType = MaterialType.Steel,
					Name = name,
				};
			}

			return new IdeaMaterialByName(id)
			{
				MaterialType = MaterialType.Steel,
				Name = name
			};
		}
	}
}