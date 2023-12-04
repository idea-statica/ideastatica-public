namespace IdeaStatiCa.CheckbotPlugin.Common.Mappers
{
	public static partial class Mapper
	{
		public static Protos.EventOpenCheckApplication Map(Models.EventOpenCheckApplication source)
		{
			return new Protos.EventOpenCheckApplication()
			{
				Object = Map(source.ModelObject)
			};
		}

		public static Models.EventOpenCheckApplication Map(Protos.EventOpenCheckApplication source)
		{
			return new Models.EventOpenCheckApplication(Map(source.Object));
		}
	}
}