using System;

namespace IdeaRS.OpenModel.Message
{
	/// <summary>
	/// Nonconformity severities
	/// </summary>
	public enum NonConformitySeverity
	{
		/// <summary>
		/// Information
		/// </summary>
		Info = 0,

		/// <summary>
		/// Deal as warning
		/// </summary>
		Warning,

		/// <summary>
		/// Error
		/// </summary>
		Error,

		/// <summary>
		/// Error, coancel of the calculation required
		/// </summary>
		TerminatedError,
	}

	/// <summary>
	/// Non-conformity issue
	/// </summary>
	public class NonConformityIssue
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public NonConformityIssue()
		{
			Guid = Guid.Empty;
			Description = string.Empty;
			Severity = NonConformitySeverity.Info;
		}

		/// <summary>
		/// Description of the nonconformity
		/// </summary>
		public Guid Guid { get; set; }

		/// <summary>
		/// Description of the nonconformity
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// severity of this nonconformity
		/// </summary>
		public NonConformitySeverity Severity { get; set; }
	}
}