using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.BimItems;
using IdeaStatiCa.Plugin;

namespace SafFeaBimLink
{
	internal class SyncItem : IBimItem
	{
		public BIMItemType Type { get; }

		public IIdeaObject ReferencedObject { get; }

		public SyncItem(BIMItemType type, IIdeaObject obj)
		{
			Type = type;
			ReferencedObject = obj;
		}
	}
}
