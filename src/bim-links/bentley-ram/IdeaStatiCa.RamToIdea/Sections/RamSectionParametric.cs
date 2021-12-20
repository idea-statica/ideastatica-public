using IdeaRS.OpenModel.CrossSection;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.RamToIdea.Model;
using System.Collections.Generic;

namespace IdeaStatiCa.RamToIdea.Sections
{
	internal class RamSectionParametric : AbstractRamSection, IIdeaCrossSectionByParameters
	{
		public IIdeaMaterial Material => GetMaterial();

		public CrossSectionType Type => _cssParams.CrossSectionType;

		public HashSet<Parameter> Parameters => new HashSet<Parameter>(_cssParams.Parameters);

		private readonly CrossSectionParameter _cssParams;

		public RamSectionParametric(double height, RamMemberProperties props, CrossSectionParameter cssParam)
			: base(height, props)
		{
			_cssParams = cssParam;
		}
	}
}