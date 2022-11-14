namespace IdeaStatiCa.PluginSystem.PluginList.Descriptors
{
	public sealed class ActionButtonDescriptor
	{
		public string Name { get; }

		public string Image { get; }

		public string Text { get; }

		public string Tooltip { get; }

		public ActionButtonDescriptor(string name, string image, string text, string tooltip)
		{
			Name = name;
			Image = image;
			Text = text;
			Tooltip = tooltip;
		}
	}
}