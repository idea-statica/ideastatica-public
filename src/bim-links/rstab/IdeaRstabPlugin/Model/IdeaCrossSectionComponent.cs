using IdeaRS.OpenModel.Geometry2D;
using IdeaStatiCa.BimApi;

namespace IdeaRstabPlugin.Model
{
	internal class IdeaCrossSectionComponent : IIdeaCrossSectionComponent
	{
		public IIdeaMaterial Material { get; set; }

		public Region2D Geometry { get; set; }

		public int Phase { get; set; }
	}
}
