using IdeaRS.OpenModel;
using IdeaStatiCa.BimApi;
using System;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal class MaterialImporter : AbstractImporter<IIdeaMaterial>
	{
		protected override OpenElementId ImportInternal(ImportContext ctx, IIdeaMaterial material)
		{
			throw new NotImplementedException();
		}
	}
}