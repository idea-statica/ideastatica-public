namespace IdeaStatiCa.BimApi
{
	public interface IIdeaCrossSectionByName : IIdeaCrossSection
	{
		/// <summary>
		/// Material of the cross-section.
		/// </summary>
		IIdeaMaterial Material { get; }
	}
}