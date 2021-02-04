
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IdeaStatiCa.BimApi
{
	public class IIdeaCrossSectionByParameters : IIdeaCrossSection
	{

		public IIdeaCrossSectionByParameters()
		{
		}

		public IIdeaMaterial material;

		public IdeaRS.OpenModel.CrossSection.CrossSectionType type;

		public HashSet<IdeaRS.OpenModel.CrossSection.Parameter> parameters;

	}
}