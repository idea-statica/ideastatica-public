using IdeaStatica.BimApiLink.Identifiers;
using IdeaStatiCa.BimApi;

namespace IdeaStatica.BimApiLink.Hooks
{
	internal class ImporterHookManager : AbstractHookManager<IImporterHook>, IImporterHook
	{
		public void ExitCreate(IIdentifier identifier, IIdeaObject ideaObject)
			=> Invoke(x => x.ExitCreate(identifier, ideaObject));

		public void EnterCreate(IIdentifier identifier)
			=> Invoke(x => x.EnterCreate(identifier));
	}
}