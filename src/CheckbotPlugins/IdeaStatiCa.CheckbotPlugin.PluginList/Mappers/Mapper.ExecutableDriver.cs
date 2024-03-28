namespace IdeaStatiCa.CheckbotPlugin.PluginList.Mappers
{
	internal static partial class Mapper
	{
		internal static Descriptors.ExecutableDriverDescriptor Map(Json.ExecutableDriver source)
		{
			return new Descriptors.ExecutableDriverDescriptor(source.Path, source.AdditionalArguments);
		}

		internal static Json.ExecutableDriver Map(Descriptors.ExecutableDriverDescriptor source)
		{
			return new Json.ExecutableDriver
			{
				Path = source.Path,
				AdditionalArguments = source.AdditionalArguments
			};
		}
	}
}