using IdeaRS.OpenModel;

namespace IdeaStatiCa.BimImporter
{
	public interface IBimImporter
	{
		OpenModelContainer ImportSelectedConnectionsToIom();
	}
}