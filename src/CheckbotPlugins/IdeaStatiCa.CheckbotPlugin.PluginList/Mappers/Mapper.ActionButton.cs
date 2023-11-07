namespace IdeaStatiCa.PluginSystem.PluginList.Mappers
{
	internal static partial class Mapper
	{
		internal static Descriptors.ActionButtonDescriptor Map(Json.ActionButton source)
		{
			return new Descriptors.ActionButtonDescriptor(source.Name, source.Image, source.Text, source.Tooltip);
		}

		internal static Json.ActionButton Map(Descriptors.ActionButtonDescriptor source)
		{
			return new Json.ActionButton
			{
				Name = source.Name,
				Image = source.Image,
				Text = source.Text,
				Tooltip = source.Tooltip
			};
		}
	}
}