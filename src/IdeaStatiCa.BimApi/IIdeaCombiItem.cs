namespace IdeaStatiCa.BimApi
{
	public interface IIdeaCombiItem : IIdeaObject
	{
		IIdeaLoadCase LoadCase { get; }

		/// <summary>
		/// Coefficient
		/// </summary>
		double Coeff { get; }

		/// <summary>
		/// Inserted another combination
		/// </summary>
		IIdeaCombiInput Combination { get; }
	}
}