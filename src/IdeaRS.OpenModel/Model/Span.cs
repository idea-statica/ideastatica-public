namespace IdeaRS.OpenModel.Model
{
	[OpenModelClass("CI.StructModel.Structure.Span,CI.StructuralElements", "CI.StructModel.Structure.ISpan,CI.BasicTypes")]
	public class Span : OpenElementId
	{
		public ReferenceElement StartCrossSection { get; set; }

		public ReferenceElement EndCrossSection { get; set; }

		public double StartPosition { get; set; }

		public double EndPosition { get; set; }
	}
}