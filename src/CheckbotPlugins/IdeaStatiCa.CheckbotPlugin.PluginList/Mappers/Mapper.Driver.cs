using IdeaStatiCa.CheckbotPlugin.PluginList.Descriptors;
using IdeaStatiCa.CheckbotPlugin.PluginList.Json;
using System;

namespace IdeaStatiCa.CheckbotPlugin.PluginList.Mappers
{
	internal static partial class Mapper
	{
		internal static Descriptors.PluginDriverDescriptor Map(Json.Driver source)
			=> source switch
			{
				DotNetRunnerDriver x => Map(x),
				ExecutableDriver x => Map(x),
				_ => throw new NotImplementedException()
			};

		internal static Json.Driver Map(Descriptors.PluginDriverDescriptor source)
			=> source switch
			{
				DotNetRunnerDriverDescriptor x => Map(x),
				ExecutableDriverDescriptor x => Map(x),
				_ => throw new NotImplementedException()
			};
	}
}