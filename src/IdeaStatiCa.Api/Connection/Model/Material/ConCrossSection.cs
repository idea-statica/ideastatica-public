namespace IdeaStatiCa.Api.Connection.Model.Material
{
	/// <summary>
	/// A cross-section of the project: its id, display name and how it is defined
	/// (library / parametric / custom). The same object is returned by the listing, by the item
	/// route and by every route that creates or replaces a section. The evaluated outline geometry
	/// is a separate resource (<c>cross-sections/{id}/geometry</c>, see <see cref="ConCrossSectionGeometry"/>).
	/// </summary>
	public class ConCrossSection
	{
		/// <summary>Id of the cross-section in the project.</summary>
		public int Id { get; set; }

		/// <summary>Display name of the cross-section.</summary>
		public string Name { get; set; }

		/// <summary>How the section is defined; the concrete subtype says which kind it is.</summary>
		public ConCrossSectionDefinition Definition { get; set; }
	}
}
