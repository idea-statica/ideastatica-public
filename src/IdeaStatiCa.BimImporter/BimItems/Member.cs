using IdeaRS.OpenModel;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;

namespace IdeaStatiCa.BimImporter.BimItems
{
	internal class Member : IBimItem
	{
		public BIMItemType Type => BIMItemType.Member;

		public IIdeaObject ReferencedObject { get; }

		private readonly IIdeaMember1D _member;

		public Member(IIdeaMember1D member)
		{
			_member = member;
		}

		public BIMItemId Create(IImportContext ctx)
		{
			ReferenceElement refElm = ctx.Import(_member);

			return new BIMItemId()
			{
				Id = refElm.Id,
				Type = BIMItemType.Member
			};
		}
	}
}