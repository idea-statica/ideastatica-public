namespace IdeaStatiCa.CheckbotPlugin.PluginList.Mappers
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
				MapActionButtons(source.CustomActions));
		}

		internal static Json.Plugin Map(Descriptors.PluginDescriptor source)
		{
			return new Json.Plugin
			{
				Type = MapPluginType(source.Type),
				Driver = Map(source.DriverDescriptor),
				Name = source.Name,
				Actions = Map(source.SystemActionsDescriptor),
				CustomActions = MapActionButtons(source.CustomActionDescriptors)
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

		private static Descriptors.ActionButtonDescriptor[]? MapActionButtons(Json.ActionButton[]? source)
		{
			if (source is null)
			{
				return null;
			}

			return source
				.Select(Map)
				.WhereNotNull()
				.ToArray();
		}

		private static Json.ActionButton[]? MapActionButtons(Descriptors.ActionButtonDescriptor[]? source)
		{
			if (source is null)
			{
				return null;
			}

			return source
				.Select(Map)
				.WhereNotNull()
				.ToArray();
		}

		internal static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source)
		{
#pragma warning disable S3267 // Loops should be simplified with "LINQ" expressions
			foreach (T? item in source)
			{
				if (item is not null)
				{
					yield return item;
				}
			}
#pragma warning restore S3267 // Loops should be simplified with "LINQ" expressions
		}
	}
}