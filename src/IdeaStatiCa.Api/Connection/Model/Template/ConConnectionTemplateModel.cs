using System;
using System.Collections.Generic;

namespace IdeaStatiCa.Api.Connection.Model
{
	public class ConConnectionTemplateModel
	{
		public string TemplateName { get; set; }
		public Guid TempalteGuid { get; set; }
		public Guid InstanceGuid { get; set; }
		public List<int> MemberIds { get; set; } = new List<int>();
		public List<int> OperationIds { get; set; } = new List<int>();
		public List<int> ParameterIds { get; set; } = new List<int>();
		public List<int> CommonPropertyIds { get; set; } = new List<int>();
	}
}
