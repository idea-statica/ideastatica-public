namespace IdeaRS.OpenModel.Model
{
	public class Span : OpenElementId
	{
		public ReferenceElement StartCrossSection { get; set; }

		public ReferenceElement EndCrossSection { get; set; }

		public double StartPosition { get; set; }

		public double EndPosition { get; set; }
	}
}