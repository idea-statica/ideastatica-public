namespace IdeaStatiCa.CheckbotPlugin.PluginList.Descriptors
{
	public sealed class SystemActionsDescriptor
	{		
		public OpenButtonDescriptor? Open { get; }

		public SystemActionsDescriptor(OpenButtonDescriptor? open)
		{
			Open = open;
		}
	}
}