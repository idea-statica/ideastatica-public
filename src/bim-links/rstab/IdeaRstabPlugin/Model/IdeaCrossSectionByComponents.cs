using IdeaStatiCa.BimApi;
using System.Collections.Generic;

namespace IdeaRstabPlugin.Model
{
	internal class IdeaCrossSectionByComponents : IIdeaCrossSectionByComponents
	{
		public HashSet<IIdeaCrossSectionComponent> Components { get; set; } = new HashSet<IIdeaCrossSectionComponent>();

		public double Rotation { get; set; }

		public string Id { get; set; }

		public string Name { get; set; }

		public bool IsInPrincipal => false;
	}
}
