using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;

namespace IdeaStatiCa.BimImporter.BimItems
{
	public class Detail : IBimItem
	{
		/// <summary>
		/// Gets the type of the bim item.
		/// </summary>
		public BIMItemType Type => BIMItemType.Detail;

		/// <summary>
		/// Gets the referenced bim object.
		/// </summary>
		public IIdeaObject ReferencedObject { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Detail"/> class.
		/// </summary>
		/// <param name="member2D">The member2D.</param>
		public Detail(IIdeaMember2D member2D)
		{
			ReferencedObject = member2D;
		}
	}
}
