using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;

namespace BimApiLinkCadExample.BimApi
{
	/// <summary>
	/// A negative plate whose material and origin node are imported when BimApiLink reads them.
	/// </summary>
	internal class NegativePlate : IdeaNegativePlate
	{
		public NegativePlate(int id)
			: base(id)
		{
		}

		public string MaterialName { get; set; }

		public string OriginNodeId { get; set; }

		public override IIdeaMaterial Material => Get<IIdeaMaterial>(MaterialName);

		public override IIdeaNode Origin => Get<IIdeaNode>(OriginNodeId);
	}
}
