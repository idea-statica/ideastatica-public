namespace IdeaStatica.BimApiLink.Scoping
{
	public abstract class ScopeAwareObject
	{
		protected IScope Scope => _scopeProvider.GetScope();

		private readonly IScopeProvider _scopeProvider;

		protected ScopeAwareObject(IScopeProvider scopeProvider)
		{
			_scopeProvider = scopeProvider;
		}

		protected ScopeAwareObject()
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