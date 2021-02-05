
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IdeaStatiCa.BimApi
{
	public interface IIdeaCrossSectionByParameters : IIdeaCrossSection
	{

		IIdeaMaterial Material { get; }

		IdeaRS.OpenModel.CrossSection.CrossSectionType Type { get; }

		HashSet<IdeaRS.OpenModel.CrossSection.Parameter> Parameters { get; }

	}
}