using System;
using System.Collections.Generic;
using IdeaStatiCa.BimApi;
using IdeaRS.OpenModel.CrossSection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaStatiCa.RamToIdea.BimApi
{
	internal class RamCrossSectionByName : IIdeaCrossSectionByName
	{
		public IIdeaMaterial Material { get; set; }

		public double Rotation { get; set; }

		public string Id { get; set; }

		public string Name { get; set; }

		public bool IsInPrincipal => false;
	}
}
