using System.Collections.Generic;
using Newtonsoft.Json;

namespace IdeaStatiCa.Api.Connection.Model
{
	public class TemplateConversions
	{
		[JsonProperty(Required = Required.Always)]
		public List<BaseTemplateConversion> Conversions { get; set; }
		public string CountryCode { get; set; }
	}
}
