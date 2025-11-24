using System.Collections.Generic;

namespace IdeaStatiCa.Api.Connection.Model
{ 
	public class ConTemplateApplyResult
	{
		/// <summary>
		/// Indicates whether the template application succeeded without critical errors
		/// </summary>
		public bool IsSuccess { get; set; }

		/// <summary>
		/// List of issues encountered during template application
		/// </summary>
		public List<ConNonConformityIssue> Issues { get; set; }

		public ConTemplateApplyResult()
		{
			Issues = new List<ConNonConformityIssue>();
			IsSuccess = true;
		}
	}
}
