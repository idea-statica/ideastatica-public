using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;

namespace IdeaStatiCa.BimImporter.BimItems
{
	public interface IBimItem
	{
		BIMItemType Type { get; }

		IIdeaObject ReferencedObject { get; }
	}
}