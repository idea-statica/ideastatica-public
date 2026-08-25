using IdeaStatiCa.BimApi;

namespace IdeaStatiCa.RamToIdea.BimApi
{
	public class RamCrossSectionByName : IIdeaCrossSectionByName
	{
		public IIdeaMaterial Material { get; set; }

		public double Rotation { get; set; }

		public string Id { get; set; }

		public string Name { get; set; }

		public bool IsInPrincipal => false;
	}
}
