using AutoMapper;
using IdeaStatiCa.CheckbotPlugin.Common.MappingProfiles;

namespace IdeaStatiCa.CheckbotPlugin.Common
{
	public static class Mapping
	{
		private static readonly Mapper _mapper = _mapper = new Mapper(new MapperConfiguration(
			   x => x.AddProfile(typeof(PluginApiProfile))));

		public static Mapper GetMapper() => _mapper;

		public static TDest Map<TDest>(object obj) => _mapper.Map<TDest>(obj);
	}
}