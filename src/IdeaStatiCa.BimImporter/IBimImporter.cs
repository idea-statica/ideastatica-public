using IdeaStatiCa.Plugin;
using System.Collections.Generic;

namespace IdeaStatiCa.BimImporter
{
	public interface IBimImporter
	{
		ModelBIM ImportConnections();

		ModelBIM ImportMembers();

		List<ModelBIM> ImportSelected(List<BIMItemsGroup> selected);
	}
}