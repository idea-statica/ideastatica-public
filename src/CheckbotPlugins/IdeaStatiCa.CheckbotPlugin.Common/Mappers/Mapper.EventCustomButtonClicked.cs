namespace IdeaStatiCa.CheckbotPlugin.Common.Mappers
{
	public static partial class Mapper
	{
		public static Protos.EventCustomButtonClicked Map(Models.EventCustomButtonClicked source)
		{
			return new Protos.EventCustomButtonClicked
			{
				ButtonName = source.ButtonName
			};
		}

		public static Models.EventCustomButtonClicked Map(Protos.EventCustomButtonClicked source)
		{
			return new Models.EventCustomButtonClicked(source.ButtonName);
		}
	}
}