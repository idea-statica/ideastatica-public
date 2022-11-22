using IdeaRS.OpenModel.CrossSection;
using IdeaStatiCa.BimApi;
using System.Collections.Generic;

namespace IdeaRstabPlugin.BimApi
{
	/// <inheritdoc cref="IIdeaCrossSectionByName"/>
	internal class RstabCrossSectionParametric : IIdeaCrossSectionByName
	{
		public double Rotation { get; set; }

		public string Id { get; set; }

		public string Name { get; set; }

		public IIdeaMaterial Material { get; set; }

		public CrossSectionType Type { get; set; }

		public HashSet<Parameter> Parameters { get; set; }

		public bool IsInPrincipal => false;
	}
}