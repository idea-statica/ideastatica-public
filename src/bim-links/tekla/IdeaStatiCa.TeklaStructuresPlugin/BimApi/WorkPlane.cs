using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;

namespace IdeaStatiCa.TeklaStructuresPlugin.BimApi
{
	internal class WorkPlane : IdeaWorkPlane
	{
		public override IIdeaNode Origin => Get<IIdeaNode>(OriginNo);

		public string OriginNo { get; set; }

		public WorkPlane(string no)
			: base(no)
		{
		}
	}
}