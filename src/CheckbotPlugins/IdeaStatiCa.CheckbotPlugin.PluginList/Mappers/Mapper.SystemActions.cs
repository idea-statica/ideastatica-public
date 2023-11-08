namespace IdeaStatiCa.PluginSystem.PluginList.Mappers
{
	internal static partial class Mapper
	{
		internal static Descriptors.SystemActionsDescriptor? Map(Json.SystemActions? source)
		{
			if (source is null)
			{
				return null;
			}

			return new Descriptors.SystemActionsDescriptor(Map(source.Open));
		}

		internal static Json.SystemActions? Map(Descriptors.SystemActionsDescriptor? source)
		{
			if (source is null)
			{
				return null;
			}

			return new Json.SystemActions
			{
				Open = Map(source.Open)
			};
		}
	}
}