namespace IdeaStatica.BimApiLink.Scoping
{
	public abstract class ImportContextAware
	{
		protected IScope Scope => _scopeProvider.GetScope();

		private readonly IScopeProvider _scopeProvider;

		protected ImportContextAware(IScopeProvider scopeProvider)
		{
			_scopeProvider = scopeProvider;
		}

		protected ImportContextAware()
			: this(new ScopeProvider())
		{
		}

		private sealed class ScopeProvider : IScopeProvider
		{
			public IScope GetScope()
				=> BimLinkScope.Current;
		}
	}
}