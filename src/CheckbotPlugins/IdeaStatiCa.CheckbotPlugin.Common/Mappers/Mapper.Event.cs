using System;

namespace IdeaStatiCa.CheckbotPlugin.Common.Mappers
{
	public static partial class Mapper
	{
		public static Protos.Event Map(Models.Event source)
		{
			Protos.Event evt = new Protos.Event();

			switch (source)
			{
				case Models.EventPluginStop x:
					evt.PluginStop = Map(x);
					break;

				case Models.EventProcedureBegin x:
					evt.ProcedureBegin = Map(x);
					break;

				case Models.EventOpenCheckApplication x:
					evt.OpenCheckApplication = Map(x);
					break;

				case Models.EventCustomButtonClicked x:
					evt.CustomButtonClicked = Map(x);
					break;

				default:
					throw new NotImplementedException();
			}

			return evt;
		}

		public static Models.Event Map(Protos.Event source)
		{
			return source.EventCase switch
			{
				Protos.Event.EventOneofCase.PluginStop => Map(source.PluginStop),
				Protos.Event.EventOneofCase.ProcedureBegin => Map(source.ProcedureBegin),
				Protos.Event.EventOneofCase.OpenCheckApplication => Map(source.OpenCheckApplication),
				Protos.Event.EventOneofCase.CustomButtonClicked => Map(source.CustomButtonClicked),
				_ => throw new NotImplementedException(),
			};
		}
	}
}