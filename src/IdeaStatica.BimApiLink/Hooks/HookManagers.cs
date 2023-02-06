namespace IdeaStatica.BimApiLink.Hooks
{
	internal class HookManagers
	{
		public ImporterHookManager ImporterHookManager { get; } = new ImporterHookManager();

		public PluginHookManager PluginHookManager { get; } = new PluginHookManager();

		public ScopeHookManager ScopeHookManager { get; } = new ScopeHookManager();
	}
}