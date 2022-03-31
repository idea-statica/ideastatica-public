namespace IdeaRS.OpenModel.Model
{
	public class Span
	{
		public ReferenceElement CrossSectionBegin { get; set; }

		public ReferenceElement CrossSectionEnd { get; set; }

		public double StartPosition { get; set; }

		public double EndPosition { get; set; }
	}
}