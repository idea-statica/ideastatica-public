namespace IdeaStatiCa.BimApi
{
	/// <summary>
	/// Represent pin grid extend IIdeaGrid
	/// </summary>
	public interface IIdeaPinGrid : IIdeaGrid
	{
		/// <summary>
		/// Pin
		/// </summary>
		IIdeaPin Pin { get; }
	}
}
