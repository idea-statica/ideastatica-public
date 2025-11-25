using System.Collections.Generic;

namespace IdeaStatiCa.Api.Connection.Model
{ 
	public class ConTemplateApplyResult
	{
		/// <summary>
		/// Indicates whether the template application succeeded without any NonConformity issues or with just information level issues
		/// </summary>
		public bool AppliedWithoutIssues { get; set; }

		/// <summary>
		/// Model of the applied template
		/// It contains Id of template and Ids of created members, operations, parameters, etc.
		/// </summary>
		public ConConnectionTemplateModel TemplateModel { get; set; }

		/// <summary>
		/// List of issues encountered during template application
		/// </summary>
		public List<ConNonConformityIssue> Issues { get; set; }
	}


}
