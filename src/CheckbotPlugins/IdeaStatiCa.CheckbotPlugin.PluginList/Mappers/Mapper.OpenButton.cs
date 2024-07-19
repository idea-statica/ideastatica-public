namespace IdeaStatiCa.CheckbotPlugin.PluginList.Mappers
{
	internal static partial class Mapper
	{
		internal static Descriptors.OpenButtonDescriptor? Map(Json.OpenButton? source)
		{
			if (source is null)
			{
				return null;
			}

			return new Descriptors.OpenButtonDescriptor(source.Name, source.Image, source.Text, source.Tooltip, source.AllowedTypologyCodes);
		}

		internal static Json.OpenButton? Map(Descriptors.OpenButtonDescriptor? source)
		{
			if (source is null)
			{
				return null;
			}

			return new Json.OpenButton
			{
				Name = source.Name,
				Image = source.Image,
				Text = source.Text,
				Tooltip = source.Tooltip,
				AllowedTypologyCodes = source.AllowedTypology,
			};
		}
	}
}
