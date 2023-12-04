namespace IdeaStatiCa.CheckbotPlugin.Common.Mappers
{
	public static partial class Mapper
	{
		public static Protos.EventPluginStop Map(Models.EventPluginStop source)
		{
			return new Protos.EventPluginStop();
		}

		public static Models.EventPluginStop Map(Protos.EventPluginStop source)
		{
			return new Models.EventPluginStop();
		}
	}
}