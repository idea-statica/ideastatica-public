using System.Collections.Generic;

namespace IdeaStatiCa.Api.Connection.Model
{
	public class ConTemplateMappingGetParam
	{
		public string Template { get; set; }
		public List<int> MemberIds { get; set; } = new List<int>();
	}
}
