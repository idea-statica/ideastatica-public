namespace IdeaStatiCa.Api.Connection.Model
{
	/// <summary>
	/// Severity levels for non-conformity issues
	/// </summary>
	public enum ConNonConformityIssueSeverity
	{
		/// <summary>
		/// Informational message
		/// </summary>
		Info = 0,

		/// <summary>
		/// Warning - should be reviewed but doesn't prevent operation
		/// </summary>
		Warning = 1,

		/// <summary>
		/// Error - indicates a problem that should be addressed
		/// </summary>
		Error = 2,

		/// <summary>
		/// Critical error - operation cannot continue
		/// </summary>
		TerminatedError = 3
	}
}
