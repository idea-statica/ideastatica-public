using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Material;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;
using System;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal class MaterialImporter : AbstractImporter<IIdeaMaterial>
	{
		public MaterialImporter(IPluginLogger logger) : base(logger)
		{
		}

		protected override OpenElementId ImportInternal(IImportContext ctx, IIdeaMaterial material)
		{
			if (material is IIdeaMaterialSteel matSteal)
			{
				MatSteel mat = matSteal.Material;

				if (mat.Name == null)
				{
					mat.Name = material.Name;
				}

				return mat;
			}

			throw new NotImplementedException();
		}
	}
}