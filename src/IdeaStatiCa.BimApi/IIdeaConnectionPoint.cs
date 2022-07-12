using System.Collections.Generic;

namespace IdeaStatiCa.BimApi
{
	public interface IIdeaConnectionPoint : IIdeaObject
	{
		IIdeaNode Node { get; }

		IEnumerable<IIdeaConnectedMember> ConnectedMembers { get; }
	}
}
