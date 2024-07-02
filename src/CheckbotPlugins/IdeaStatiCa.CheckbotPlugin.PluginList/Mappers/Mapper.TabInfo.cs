namespace IdeaStatiCa.CheckbotPlugin.PluginList.Mappers
{
	internal static partial class Mapper
	{
		internal static Descriptors.TabInfoDescriptor Map(Json.TabInfo source)
		{
			return new Descriptors.TabInfoDescriptor(source.TabName);
		}

		internal static Json.TabInfo Map(Descriptors.TabInfoDescriptor source)
		{
			return new Json.TabInfo
			{
				TabName = source.TabName
			};
		}
	}
}
