namespace IdeaStatiCa.BimApi
{
	public interface IIdeaSpan : IIdeaObject
	{
		/// <summary>
		/// Cross-section at the start of the element.
		/// </summary>
		IIdeaCrossSection StartCrossSection { get; }

		/// <summary>
		/// Cross-section at the and of the element.
		/// </summary>
		IIdeaCrossSection EndCrossSection { get; }

		double StartPosition { get; }

		double EndPosition { get; }
	}
}