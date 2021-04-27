using IdeaRS.OpenModel;
using IdeaStatiCa.BimApi;
using System;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal class MaterialImporter : AbstractImporter<IIdeaMaterial>
	{
		protected override OpenElementId ImportInternal(IImportContext ctx, IIdeaMaterial material)
		{
			if (material is IIdeaMaterialSteel matSteal)
			{
				string name = material.Name;
				if (name != null)
				{
					matSteal.Material.Name = name;
				}

				return matSteal.Material;
			}

			throw new NotImplementedException();
		}
	}
}