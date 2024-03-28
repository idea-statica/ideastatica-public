namespace IdeaStatiCa.CheckbotPlugin.PluginList.Mappers
{
	internal static partial class Mapper
	{
		internal static Descriptors.DotNetRunnerDriverDescriptor Map(Json.DotNetRunnerDriver source)
		{
			return new Descriptors.DotNetRunnerDriverDescriptor(source.Path, source.ClassName);
		}

		internal static Json.DotNetRunnerDriver Map(Descriptors.DotNetRunnerDriverDescriptor source)
		{
			return new Json.DotNetRunnerDriver
			{
				Path = source.Path,
				ClassName = source.ClassName
			};
		}
	}
}