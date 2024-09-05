namespace IdeaStatiCa.BimApi
{
	/// <summary>
	/// Represent pin grid extend IIdeaFastenerGrid
	/// </summary>
	public interface IIdeaPinGrid : IIdeaFastenerGrid
	{
		/// <summary>
		/// Pin
		/// </summary>
		IIdeaPin Pin { get; }
	}
}
