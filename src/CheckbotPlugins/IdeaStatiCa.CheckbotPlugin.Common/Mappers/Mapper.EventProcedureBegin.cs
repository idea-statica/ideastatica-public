namespace IdeaStatiCa.CheckbotPlugin.Common.Mappers
{
	public static partial class Mapper
	{
		public static Protos.EventProcedureBegin Map(Models.EventProcedureBegin source)
		{
			return new Protos.EventProcedureBegin();
		}

		public static Models.EventProcedureBegin Map(Protos.EventProcedureBegin source)
		{
			return new Models.EventProcedureBegin();
		}
	}
}