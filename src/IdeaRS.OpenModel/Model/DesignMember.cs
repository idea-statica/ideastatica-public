using System.Collections.Generic;

namespace IdeaRS.OpenModel.Model
{

	/// <summary>
	/// DesignMember
	/// </summary>
	[OpenModelClass("CI.Common.CheckMember.DesignMember,CI.ServicesCommon", "CI.Common.CheckMember.IDesignMember,CI.BasicTypes")]
	public class DesignMember : OpenElementId
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public DesignMember()
		{
			Members = new List<ReferenceElement>();
		}

		/// <summary>
		/// Name of Element
		/// </summary>
		public System.String Name { get; set; }

		/// <summary>
		/// Array of Member1D
		/// </summary>
		/// <returns>List of Member1D</returns>
		public List<ReferenceElement> Members { get; set; }

	}
}