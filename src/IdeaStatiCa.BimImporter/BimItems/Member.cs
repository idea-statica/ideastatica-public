using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;

namespace IdeaStatiCa.BimImporter.BimItems
{
	/// <summary>
	/// Member bim item.
	/// </summary>
	/// <seealso cref="IdeaStatiCa.BimImporter.BimItems.IBimItem" />
	public class Member : IBimItem
	{
		/// <summary>
		/// Gets the type of the bim item.
		/// </summary>
		public BIMItemType Type => BIMItemType.Member;

		/// <summary>
		/// Gets the referenced bim object.
		/// </summary>
		public IIdeaObject ReferencedObject { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Member"/> class.
		/// </summary>
		/// <param name="member">The member.</param>
		public Member(IIdeaMember1D member)
		{
			ReferencedObject = member;
		}
	}
}