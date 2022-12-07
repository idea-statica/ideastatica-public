using IdeaStatica.BimApiLink.Identifiers;
using IdeaStatiCa.BimApi;
using System;
using System.Collections.Generic;

namespace IdeaStatica.BimApiLink.Plugin
{
	public class CadUserSelection
	{
		public ICollection<Identifier<IIdeaObject>> Objects { get; set; }
			= Array.Empty<Identifier<IIdeaObject>>();

		public ICollection<Identifier<IIdeaMember1D>> Members { get; set; }
			= Array.Empty<Identifier<IIdeaMember1D>>();

		public ICollection<Identifier<IIdeaConnectionPoint>> ConnectionPoints { get; set; }
			= Array.Empty<Identifier<IIdeaConnectionPoint>>();

	}
}
