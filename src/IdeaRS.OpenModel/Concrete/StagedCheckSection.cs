using IdeaRS.OpenModel.Concrete.Load;

namespace IdeaRS.OpenModel.Concrete
{
	/// <summary>
	/// Single check section
	/// </summary>
	[OpenModelClass("CI.Concrete.SingleCheckProject.StagedSection,CI.SingleCheckProject", "CI.Services.Concrete.ServicePoint.ISingleCheckSection,CI.ServiceTypes", typeof(CheckSection))]
	public class StagedCheckSection : CheckSection
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public StagedCheckSection()
			: base()
		{
		}

		/// <summary>
		/// Stages loading
		/// </summary>
		public StagesLoading StagesLoading { get; set; }
	}
}