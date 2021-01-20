using System.Collections.Generic;

namespace IdeaRS.OpenModel.Model
{
	/// <summary>
	/// Representation of member2D
	/// </summary>
	[OpenModelClass("CI.StructModel.Structure.Member2D,CI.StructuralElements", "CI.StructModel.Structure.IMember2D,CI.BasicTypes")]
	public class Member2D : OpenElementId
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public Member2D()
		{
			Elements2D = new List<ReferenceElement>();
			Members1D = new List<ReferenceElement>();
		}

		/// <summary>
		/// Name of Member
		/// </summary>
		public System.String Name { get; set; }

		/// <summary>
		/// Array of element2D
		/// </summary>
		/// <returns>List of Element2D</returns>
		public List<ReferenceElement> Elements2D { get; set; }

		/// <summary>
		/// Array of internal members, ribs
		/// </summary>
		public List<ReferenceElement> Members1D { get; set; }
	}
}
