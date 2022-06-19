using System.Runtime.Serialization;

namespace IdeaStatiCa.PluginSystem.PluginList.Json
{
	internal enum JsonPluginType
	{
		[EnumMember(Value = "import")]
		Import,

		[EnumMember(Value = "check")]
		Check
	}
}