using IdeaRS.OpenModel;

namespace IdeaStatica.BimApiLink.Hooks
{
	internal class ScopeHookManager : AbstractHookManager<IScopeHook>, IScopeHook
	{
		public void PreScope()
			=> Invoke(x => x.PreScope());

		public void PostScope()
			=> Invoke(x => x.PostScope());
	}
}