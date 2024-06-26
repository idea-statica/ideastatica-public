using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace IdeaStatiCa.Plugin.Api.ConnectionRest.Model.Model_Connection
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum ConLoadEffectPositionEnum
	{	
		End = 0,
		// Represents isBegin = true
		Begin = 1,	
	}
}