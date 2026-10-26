using IdeaStatiCa.BimApi;

namespace IdeaRstabPlugin.BimApi
{
	/// <inheritdoc cref="IIdeaCrossSectionByName"/>
	internal class RstabCrossSectionByName : IIdeaCrossSectionByName
	{
		public double Rotation { get; set; }

		public string Id { get; set; }

		public string Name { get; set; }

		public IIdeaMaterial Material { get; set; }

		public bool IsInPrincipal => false;
	}
}
