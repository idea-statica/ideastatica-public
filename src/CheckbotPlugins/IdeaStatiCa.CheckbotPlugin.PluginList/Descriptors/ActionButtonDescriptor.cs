namespace IdeaStatiCa.CheckbotPlugin.PluginList.Descriptors
{
	public sealed class ActionButtonDescriptor
	{
		/// <summary>
		/// Action name.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Path to the image or base64 encoded image.
		/// Supported formats are PNG, JPG, BMP.
		/// 
		/// To encode 
		/// </summary>
		public string Image { get; }

		/// <summary>
		/// Text displayed on the button in Checkbot.
		/// </summary>
		public string Text { get; }

		/// <summary>
		/// Tooltip of the button in Checkbot.
		/// </summary>
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