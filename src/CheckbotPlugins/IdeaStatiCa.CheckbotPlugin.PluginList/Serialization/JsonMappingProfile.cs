using AutoMapper;
using IdeaStatiCa.PluginSystem.PluginList.Descriptors;
using IdeaStatiCa.PluginSystem.PluginList.Json;

namespace IdeaStatiCa.PluginSystem.PluginList.Serialization
{
	internal class JsonMappingProfile : Profile
	{
		public JsonMappingProfile()
		{
			CreateMap<DotNetRunnerDriver, DotNetRunnerDriverDescriptor>()
				.ReverseMap();
			CreateMap<ExecutableDriver, ExecutableDriverDescriptor>()
				.ReverseMap();

			CreateMap<PluginDriverDescriptor, Driver>()
				.ForMember(x => x.Type, x => x.Ignore())
				.Include<DotNetRunnerDriverDescriptor, DotNetRunnerDriver>()
				.Include<ExecutableDriverDescriptor, ExecutableDriver>()
				.ReverseMap();

			CreateMap<ActionButtonDescriptor, ActionButton>()
				.ReverseMap();

			CreateMap<SystemActionsDescriptor, SystemActions>()
				.ReverseMap();

			CreateMap<PluginDescriptor, Plugin>()
				.ForMember(x => x.Driver, x => x.MapFrom(y => y.DriverDescriptor))
				.ForMember(x => x.Actions, x => x.MapFrom(y => y.SystemActionsDescriptor))
				.ForMember(x => x.CustomActions, x => x.MapFrom(y => y.CustomActionDescriptors))
				.ReverseMap()
				.ForCtorParam("driverDescriptor", x => x.MapFrom(y => y.Driver))
				.ForCtorParam("systemActionsDescriptor", x => x.MapFrom(y => y.Actions))
				.ForCtorParam("customActionDescriptors", x => x.MapFrom(y => y.CustomActions));
		}
	}
}