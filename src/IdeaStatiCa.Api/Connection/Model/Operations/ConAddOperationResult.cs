#nullable enable annotations

using System.Collections.Generic;

namespace IdeaStatiCa.Api.Connection.Model.Operations
{
	/// <summary>
	/// Response wrapper for add-operation endpoints.
	/// Contains the created operation plus any non-conformity issues produced during
	/// the model rebuild (e.g., "plate intersects supporting edge").
	/// Mirrors the pattern used by template application (<c>ConTemplateApplyResult</c>).
	/// </summary>
	/// <remarks>
	/// The operation is always stored — a non-empty <see cref="Issues"/> list does not mean it was rejected.
	/// </remarks>
	public class ConAddOperationResult
	{
		/// <summary>
		/// The <see cref="ConNonConformityIssue.Details"/> value on the entry the service synthesizes when the
		/// operation could not be checked. Match on this to tell "no verdict is available" from a nonconformity
		/// the engine reported; the description is prose and may be reworded or localized.
		/// </summary>
		public const string NotCheckedDetails = "verdict-unavailable";

		/// <summary>
		/// The created operation.
		/// </summary>
		public ConOperation Operation { get; set; } = null!;

		/// <summary>
		/// Non-conformity issues (warnings / errors) the model rebuild reported for this operation.
		/// </summary>
		/// <remarks>
		/// One entry is not produced by the rebuild: when the connection could not be checked at all — the
		/// rebuild failed, or it left no result to read — the service adds a Warning carrying
		/// <see cref="NotCheckedDetails"/>. Those two states differ — "the engine found a problem" versus "no
		/// verdict is available" — and both leave <see cref="AddedWithoutIssues"/> false.
		/// </remarks>
		public List<ConNonConformityIssue> Issues { get; set; } = new List<ConNonConformityIssue>();

		/// <summary>
		/// True when the connection was regenerated and reported no warnings or errors for this operation
		/// (only Info-level entries, if any). False also covers the case where no verdict could be produced,
		/// so it never claims a check that did not happen.
		/// </summary>
		public bool AddedWithoutIssues { get; set; } = true;
	}
}
