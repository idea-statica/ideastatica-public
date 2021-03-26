using IdeaRS.OpenModel;
using IdeaStatiCa.Plugin;

namespace IdeaStatiCa.BimImporter
{
	public interface IBimImporter
	{
		ModelBIM ImportSelectedConnectionsToIom();
	}
}