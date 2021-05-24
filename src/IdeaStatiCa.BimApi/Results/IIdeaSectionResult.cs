using IdeaRS.OpenModel.Result;

namespace IdeaStatiCa.BimApi.Results
{
	public interface IIdeaSectionResult
	{
		/// <summary>
		/// Replacement for <see cref="SectionResultBase.Loading"/>
		/// </summary>
		IIdeaLoadCase LoadCase { get; }

		SectionResultBase Result { get; }
	}
}