namespace IdeaStatiCa.PluginSystem.PluginList.Mappers
{
	internal static partial class Mapper
	{
		internal static Descriptors.SystemActionsDescriptor Map(Json.SystemActions source)
		{
			return new Descriptors.SystemActionsDescriptor(new Descriptors.ActionButtonDescriptor(source.Open.Name, source.Open.Image, source.Open.Text, source.Open.Tooltip));
		}

		internal static Json.SystemActions Map(Descriptors.SystemActionsDescriptor source)
		{
			return new Json.SystemActions
			{
				Open = MapActionButton(source.Open)
			};
		}

		private static Json.ActionButton MapActionButton(Descriptors.ActionButtonDescriptor source)
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