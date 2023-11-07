using System.Linq;

namespace IdeaStatiCa.PluginSystem.PluginList.Mappers
{


	internal static partial class Mapper
	{
		internal static Descriptors.PluginDescriptor Map(Json.Plugin source)
		{
			return new Descriptors.PluginDescriptor(
				source.Name,
				MapPluginType(source.Type),
				Map(source.Driver),
				Map(source.Actions),
				source.CustomActions.Select(Map).ToArray());
		}

		internal static Json.Plugin Map(Descriptors.PluginDescriptor source)
		{
			return new Json.Plugin
			{
				Type = MapPluginType(source.Type),
				Driver = Map(source.DriverDescriptor),
				Name = source.Name,
				Actions = Map(source.SystemActionsDescriptor),
				CustomActions = source.CustomActionDescriptors.Select(Map).ToArray()
			};
		}

		private static Descriptors.PluginType MapPluginType(Json.JsonPluginType source)
		{
			return source switch
			{
				Json.JsonPluginType.Import => Descriptors.PluginType.Import,
				Json.JsonPluginType.Check => Descriptors.PluginType.Check,
				_ => throw new System.ComponentModel.InvalidEnumArgumentException(nameof(source), (int)source, typeof(Json.JsonPluginType))
			};
		}

		private static Json.JsonPluginType MapPluginType(Descriptors.PluginType source)
		{
			return source switch
			{
				Descriptors.PluginType.Import => Json.JsonPluginType.Import,
				Descriptors.PluginType.Check => Json.JsonPluginType.Check,
				_ => throw new System.ComponentModel.InvalidEnumArgumentException(nameof(source), (int)source, typeof(Descriptors.PluginType))
			};
		}
	}
}