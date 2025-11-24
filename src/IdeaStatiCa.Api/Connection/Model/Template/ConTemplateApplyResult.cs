using System.Collections.Generic;

namespace IdeaStatiCa.Api.Connection.Model
{ 
	public class ConTemplateApplyResult
	{
		/// <summary>
		/// Indicates whether the template application succeeded without  any NonConformity issues or with just information level issues
		/// </summary>
		public bool IsSuccess { get; set; }

		/// <summary>
		/// List of issues encountered during template application
		/// </summary>
		public List<ConNonConformityIssue> Issues { get; set; }
	}
}
