using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;

namespace BimApiLinkCadExample.BimApi
{
	/// <summary>
	/// A plate whose material and origin node are imported when BimApiLink reads them.
	/// </summary>
	internal class Plate : IdeaPlate
	{
		public Plate(int id)
			: base(id)
		{
		}

		public string MaterialName { get; set; }

		public string OriginNodeId { get; set; }

		public override IIdeaMaterial Material => Get<IIdeaMaterial>(MaterialName);

		public override IIdeaNode Origin => Get<IIdeaNode>(OriginNodeId);
	}
}
