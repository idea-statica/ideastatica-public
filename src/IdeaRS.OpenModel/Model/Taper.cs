using System.Collections.Generic;

namespace IdeaRS.OpenModel.Model
{
	/// <summary>
	/// Defines haunches (variyng cross-sections) along the member.
	/// 
	/// One <see cref="Taper"/> may be assigned to multiple <see cref="Member1D">Members</see>.
	/// Sections of the member not covered by a span will use the member's cross-section.
	/// </summary>
	[OpenModelClass("CI.StructModel.Structure.MemberTapered,CI.StructuralElements", "CI.StructModel.Structure.IMemberTapered,CI.BasicTypes")]
	public class Taper : OpenElementId
	{
		/// <summary>
		/// List of <see cref="Span">spans</see>. Spans mut not overlap.
		/// </summary>
		public List<ReferenceElement> Spans { get; set; }
	}
}