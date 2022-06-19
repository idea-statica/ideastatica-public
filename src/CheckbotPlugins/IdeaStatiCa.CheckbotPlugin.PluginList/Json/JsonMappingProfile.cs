using AutoMapper;
using IdeaStatiCa.PluginSystem.PluginList.Descriptors;

namespace IdeaStatiCa.PluginSystem.PluginList.Json
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

			CreateMap<PluginDescriptor, Plugin>()
				.ForMember(x => x.Driver, x => x.MapFrom(y => y.DriverDescriptor))
				.ReverseMap()
				.ForCtorParam("driverDescriptor", x => x.MapFrom(y => y.Driver));
		}
	}
}