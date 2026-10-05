using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;
using System.Collections.Generic;

namespace IdeaStatiCa.BimApiLink.Hooks
{
	/// <summary>
	/// Called only on an <see cref="IPluginHook"/> that also implements it: after <see cref="IPluginHook.EnterImport"/>
	/// of a synchronization, with the members of all synchronized items, before any of the items is imported.
	/// </summary>
	public interface ISynchronizationHook
	{
		void EnterSynchronization(IReadOnlyCollection<Identifier<IIdeaMember1D>> members);
	}
}
