using AutoMapper;
using System;

namespace IdeaStatiCa.CheckbotPlugin.Common.MappingProfiles
{
	public class PluginApiProfile : Profile
	{
		public PluginApiProfile()
		{
			CreateMap<Protos.SettingsValue, Models.SettingsValue>()
				.ReverseMap();
			CreateMap<Protos.ModelObject, Models.ModelObject>()
				.ReverseMap();
			CreateMap<Protos.ProjectInfoResp, Models.ProjectInfo>()
				.ReverseMap();
			CreateMap<Protos.ModelExportOptions, Models.ModelExportOptions>()
				.ReverseMap();

			CreateMap<Protos.EventPluginStop, Models.EventPluginStop>()
				.ReverseMap();
			CreateMap<Protos.EventProcedureBegin, Models.EventProcedureBegin>()
				.ReverseMap();
			CreateMap<Protos.EventOpenCheckApplication, Models.EventOpenCheckApplication>()
				.ForCtorParam("modelObject", x => x.MapFrom(y => y.Object))
				.ReverseMap()
				.ForMember(x => x.Object, x => x.MapFrom(y => y.ModelObject));
			CreateMap<Protos.EventCustomButtonClicked, Models.EventCustomButtonClicked>()
				.ForCtorParam("buttonName", x => x.MapFrom(y => y.ButtonName))
				.ReverseMap()
				.ForMember(x => x.ButtonName, x => x.MapFrom(y => y.ButtonName));

			CreateMap<Protos.Event, Models.Event>()
				.ConstructUsing(Proto2ModelEventConverter)
				.ForAllMembers(x => x.Ignore());

			CreateMap<Models.Event, Protos.Event>()
				.ConstructUsing(Model2ProtoEventConverter)
				.ForAllMembers(x => x.Ignore());
		}

		private static Models.Event Proto2ModelEventConverter(Protos.Event sourceMember, ResolutionContext context)
		{
			switch (sourceMember.EventCase)
			{
				case Protos.Event.EventOneofCase.PluginStop:
					return context.Mapper.Map<Models.EventPluginStop>(sourceMember.PluginStop);

				case Protos.Event.EventOneofCase.ProcedureBegin:
					return context.Mapper.Map<Models.EventProcedureBegin>(sourceMember.ProcedureBegin);

				case Protos.Event.EventOneofCase.OpenCheckApplication:
					return context.Mapper.Map<Models.EventOpenCheckApplication>(sourceMember.OpenCheckApplication);

				case Protos.Event.EventOneofCase.CustomButtonClicked:
					return context.Mapper.Map<Models.EventCustomButtonClicked>(sourceMember.CustomButtonClicked);
			}

			throw new NotImplementedException();
		}

		private static Protos.Event Model2ProtoEventConverter(Models.Event sourceMember, ResolutionContext context)
		{
			Protos.Event evt = new Protos.Event();

			switch (sourceMember)
			{
				case Models.EventPluginStop pluginStop:
					evt.PluginStop = context.Mapper.Map<Protos.EventPluginStop>(pluginStop);
					break;

				case Models.EventProcedureBegin procedureBegin:
					evt.ProcedureBegin = context.Mapper.Map<Protos.EventProcedureBegin>(procedureBegin);
					break;

				case Models.EventOpenCheckApplication openCheckApplication:
					evt.OpenCheckApplication = context.Mapper.Map<Protos.EventOpenCheckApplication>(openCheckApplication);
					break;

				case Models.EventCustomButtonClicked customButtonClicked:
					evt.CustomButtonClicked = context.Mapper.Map<Protos.EventCustomButtonClicked>(customButtonClicked);
					break;

				default:
					throw new NotImplementedException();
			}

			return evt;
		}
	}
}