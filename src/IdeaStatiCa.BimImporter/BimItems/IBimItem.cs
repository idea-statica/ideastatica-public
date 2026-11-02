using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;

namespace IdeaStatiCa.BimImporter.BimItems
{
	/// <summary>
	/// Interface IBimItem
	/// </summary>
	public interface IBimItem
	{
		/// <summary>
		/// Gets the type of the bim item.
		/// </summary>
		BIMItemType Type { get; }

		/// <summary>
		/// Gets the referenced bim object.
		/// </summary>
		IIdeaObject ReferencedObject { get; }
	}
}