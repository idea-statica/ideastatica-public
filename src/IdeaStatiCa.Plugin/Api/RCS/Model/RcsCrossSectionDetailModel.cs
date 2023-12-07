using IdeaRS.OpenModel.CrossSection;
using IdeaRS.OpenModel.Model;

namespace IdeaStatiCa.Plugin.Api.RCS.Model
{
	public class RcsCrossSectionDetailModel
	{
		public int Id { get; set; }

		public string Description { get; set; }

		//CheckMember is abstract class that cannot be serialized
		public CheckMember1D CheckMember { get; set; }

		public int DesignMemberSpanIndex { get; set; }

		public int DesignMemberTimeAxisIndex { get; set; }

		public ReinforcedCrossSection ReinfSection { get; set; }
	}
}
