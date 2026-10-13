namespace IdeaStatiCa.IOM.VersioningService.Exceptions
{
	/// <summary>
	/// A connection element that names its material by a name the model does not list.
	/// </summary>
	public sealed class UnresolvedMaterialName
	{
		public UnresolvedMaterialName(ConnectionElementKind elementKind, string elementId, string elementName, string originalModelId, string materialName)
		{
			ElementKind = elementKind;
			ElementId = elementId;
			ElementName = elementName;
			OriginalModelId = originalModelId;
			MaterialName = materialName;
		}

		public ConnectionElementKind ElementKind { get; }

		/// <summary>
		/// The element's IOM Id, unique only within its connection.
		/// </summary>
		public string ElementId { get; }

		public string ElementName { get; }

		/// <summary>
		/// The id the producing program gave the element, or <see langword="null"/> when the file carries none.
		/// </summary>
		public string OriginalModelId { get; }

		/// <summary>
		/// The material name the element gives, which the model does not list.
		/// </summary>
		public string MaterialName { get; }
	}
}
