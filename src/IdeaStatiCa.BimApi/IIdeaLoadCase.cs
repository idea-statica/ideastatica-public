
namespace IdeaStatiCa.BimApi
{
	public interface IIdeaLoadCase : IIdeaObject
	{
		IdeaRS.OpenModel.Loading.LoadCaseType LoadType { get; set; }
	}
}
