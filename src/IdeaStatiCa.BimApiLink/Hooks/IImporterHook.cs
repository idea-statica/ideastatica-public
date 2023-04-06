using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.BimApi;

namespace IdeaStatiCa.BimApiLink.Hooks
{
	public interface IImporterHook
	{
		void EnterCreate(IIdentifier identifier);

		void ExitCreate(IIdentifier identifier, IIdeaObject ideaObject);
	}
}