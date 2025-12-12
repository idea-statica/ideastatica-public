using System;
using System.Collections.Generic;
using System.Text;

namespace IdeaStatiCa.Api.Connection.Model
{
	public class ConNonConformityIssue
	{
		/// <summary>
		/// ID of the object related to this issue (if applicable)
		/// </summary>
		public int OperationId { get; set; }
		/// <summary>
		/// Description of the issue (title)
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Detailed description of the issue
		/// </summary>
		public string Details { get; set; }

		/// <summary>
		/// Severity level: 0=Info, 1=Warning, 2=Error, 3=TerminatedError
		/// </summary>
		public ConNonConformityIssueSeverity Severity { get; set; }

	}
}
