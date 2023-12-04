namespace IdeaStatiCa.CheckbotPlugin.Common.Mappers
{
	public static partial class Mapper
	{
		public static Protos.SettingsValue Map(Models.SettingsValue source)
		{
			return new Protos.SettingsValue
			{
				Name = source.Name,
				Value = source.Value
			};
		}

		public static Models.SettingsValue Map(Protos.SettingsValue source)
		{
			return new Models.SettingsValue(source.Name, source.Value);
		}
	}
}