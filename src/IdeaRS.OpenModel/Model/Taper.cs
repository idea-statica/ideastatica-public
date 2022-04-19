using System.Collections.Generic;

namespace IdeaRS.OpenModel.Model
{
	[OpenModelClass("CI.StructModel.Structure.MemberTapered,CI.StructuralElements", "CI.StructModel.Structure.IMemberTapered,CI.BasicTypes")]
	public class Taper : OpenElementId
	{
		/// <summary>
		/// List of <see cref="Span"/>.
		/// </summary>
		public List<ReferenceElement> Spans { get; set; }
	}
}