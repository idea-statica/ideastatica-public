namespace IdeaStatiCa.BimApiLink.Persistence
{
	/// <summary>
	/// A no-op <see cref="IProjectStorage"/>: the id map is neither read from nor written to disk. Opt-in via
	/// <see cref="BimLink.WithProjectStorage"/> for hosts that own the map elsewhere and don't want an on-disk
	/// bimapi-data.json — e.g. the BIM Link Hub, where Model Coordinator owns the map and re-seeds the link via
	/// <see cref="IdeaStatiCa.BimImporter.IBimIdMapAccess"/>. Not the default; JSON persistence stays the default.
	/// </summary>
	public sealed class InMemoryProjectStorage : IProjectStorage
	{
		public void Load()
		{
		}

		public void Save()
		{
		}

		public bool IsValid() => false;
	}
}
