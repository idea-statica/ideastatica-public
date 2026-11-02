using Newtonsoft.Json;

namespace IdeaStatiCa.Api.Connection.Model
{
	public class ConTemplateApplyParam
	{
		[JsonProperty(Required = Required.Always)]
		public string ConnectionTemplate { get; set; }
		[JsonProperty(Required = Required.Always)]
		public TemplateConversions Mapping { get; set; }
	}
}
