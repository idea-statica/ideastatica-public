using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.BimItems;
using IdeaStatiCa.Plugin;
using System.Collections.Generic;

namespace IdeaStatiCa.BimImporter
{
	public interface IBimObjectImporter
	{
		ModelBIM Import(IEnumerable<IIdeaObject> objects, IEnumerable<IBimItem> bimItems,  IProject project);
	}
}