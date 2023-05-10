namespace IdeaStatiCa.BimApi
{
	/// <summary>
	/// A load, only four subtypes are supported: load cases <see cref="IIdeaLoadCase"/>, combinations <see cref="IIdeaCombiInput"/>, load on line <see cref="IIdeaLoadOnLine"/>, point load on line <see cref="IIdeaPointLoadOnLine"/>
	/// </summary>
	public interface IIdeaLoading : IIdeaObject
	{
	}
}