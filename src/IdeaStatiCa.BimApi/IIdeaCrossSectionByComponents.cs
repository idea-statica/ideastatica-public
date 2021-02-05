
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IdeaStatiCa.BimApi
{
	public interface IIdeaCrossSectionByComponents : IIdeaCrossSection
	{

		HashSet<IIdeaCrossSectionComponent> Components { get; }


	}
}