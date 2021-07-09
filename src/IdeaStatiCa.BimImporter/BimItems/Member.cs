using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;

namespace IdeaStatiCa.BimImporter.BimItems
{
	public class Member : IBimItem
	{
		public BIMItemType Type => BIMItemType.Member;

		public IIdeaObject ReferencedObject { get; }

		public Member(IIdeaMember1D member)
		{
			ReferencedObject = member;
		}
	}
}