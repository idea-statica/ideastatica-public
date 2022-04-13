using System.Collections.Generic;

namespace IdeaRS.OpenModel.Model
{
	public class Taper : OpenElementId
	{
		/// <summary>
		/// List of <see cref="Span"/>.
		/// </summary>
		public List<ReferenceElement> Spans { get; set; }
	}
}