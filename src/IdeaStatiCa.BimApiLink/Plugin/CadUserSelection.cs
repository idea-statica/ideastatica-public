using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.BimApi;
using System;
using System.Collections.Generic;

namespace IdeaStatiCa.BimApiLink.Plugin
{
	public class CadUserSelection
	{
		public ICollection<IIdentifier> Objects { get; set; }
			= Array.Empty<IIdentifier>();

		public ICollection<Identifier<IIdeaConnectedMember>> Members { get; set; }
			= Array.Empty<Identifier<IIdeaConnectedMember>>();

		public Identifier<IIdeaConnectionPoint> ConnectionPoint { get; set; }

	}
}
