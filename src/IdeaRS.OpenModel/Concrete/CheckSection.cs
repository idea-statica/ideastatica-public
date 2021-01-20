using System.Collections.Generic;
using System.Xml.Serialization;

namespace IdeaRS.OpenModel.Concrete
{
	/// <summary>
	/// Check section base class
	/// </summary>
	[XmlInclude(typeof(StandardCheckSection))]
	[XmlInclude(typeof(StagedCheckSection))]
	public abstract class CheckSection : OpenElementId
	{
		/// <summary>
		/// Constructor
		/// </summary>
		protected CheckSection()
		{
			Description = string.Empty;
			//Loads = new LoadInSection();
			Extremes = new List<CheckSectionExtreme>();
		}

		/// <summary>
		/// Description of this section
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// CheckMember
		/// </summary>
		public ReferenceElement CheckMember { get; set; }

		///// <summary>
		///// index of design member span to find beam and column data
		///// </summary>
		//int DesignMemberSpanIndex { get; set; }

		///// <summary>
		///// index of design member time axis indes for time axis data
		///// </summary>
		//int DesignMemberTimeAxisIndex { get; set; }

		/// <summary>
		/// Reinforcement of cross-section
		/// </summary>
		public ReferenceElement ReinfSection { get; set; }

		///// <summary>
		///// Loads in section
		///// </summary>
		//public LoadInSection Loads { get; set; }

		/// <summary>
		/// Section extremes
		/// </summary>
		public List<CheckSectionExtreme> Extremes { get; set; }
	}
}