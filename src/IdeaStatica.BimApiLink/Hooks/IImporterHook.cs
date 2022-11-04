using IdeaStatica.BimApiLink.Identifiers;
using IdeaStatiCa.BimApi;

namespace IdeaStatica.BimApiLink.Hooks
{
	public interface IImporterHook
	{
		void EnterCreate(IIdentifier identifier);

		void ExitCreate(IIdentifier identifier, IIdeaObject ideaObject);
	}
}