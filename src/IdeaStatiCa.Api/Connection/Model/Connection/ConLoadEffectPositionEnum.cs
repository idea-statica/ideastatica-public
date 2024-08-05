using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace IdeaStatiCa.Api.Connection.Model
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum ConLoadEffectPositionEnum
	{	
		End = 0,
		// Represents isBegin = true
		Begin = 1,	
	}
}