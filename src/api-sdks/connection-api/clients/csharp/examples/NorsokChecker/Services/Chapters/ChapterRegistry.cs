namespace NorsokChecker.Services.Chapters
{
	/// <summary>
	/// The chapters this app knows about — the one list, read by everything that needs to know.
	///
	/// Before this, a chapter was described in four places that had to agree and had no way of
	/// noticing when they did not: a checkbox in the XAML, a branch in the run, a string-prefix test
	/// in the results router (`Section.StartsWith("6.4")`) and a four-entry array in the report
	/// generator. Adding a chapter meant finding all four; missing the last one dropped its rows
	/// into "Other Checks" with nothing to indicate anything had gone wrong.
	///
	/// Registering a chapter here is what makes it exist. Nothing else needs editing — which is the
	/// property ChapterRegistryTests holds the app to.
	/// </summary>
	internal static class ChapterRegistry
	{
		/// <summary>
		/// Every chapter, in the order they appear as toggles and as report sections.
		///
		/// Built once. The chapters are stateless — everything they need for a connection comes in
		/// through <see cref="ChapterContext"/> — so one instance each is enough.
		/// </summary>
		internal static IReadOnlyList<IChapter> All { get; } = new IChapter[]
		{
			new Chapter64(),
		};

		/// <summary>The chapter with this key, or null. Keys are stable; see <see cref="IChapter.Key"/>.</summary>
		internal static IChapter? ByKey(string key) =>
			All.FirstOrDefault(c => string.Equals(c.Key, key, StringComparison.Ordinal));

		/// <summary>
		/// Which chapter a result row belongs to, from its Section.
		///
		/// A row's Section is the chapter key or something beginning with it ("6.4", "6.4.3"), which
		/// is how the norm numbers its own subsections. Matching on the registry rather than on a
		/// literal is the point: the router no longer has to be told about a new chapter.
		/// </summary>
		internal static IChapter? ForSection(string? section) =>
			string.IsNullOrEmpty(section)
				? null
				: All.FirstOrDefault(c => section!.StartsWith(c.Key, StringComparison.Ordinal));
	}
}
