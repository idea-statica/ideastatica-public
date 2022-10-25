namespace IdeaStatica.BimApiLink.Hooks
{
	internal class HookManagers
	{
		public ImporterHookManager ImporterHookManager { get; } = new();

		public PluginHookManager PluginHookManager { get; } = new();
	}
}