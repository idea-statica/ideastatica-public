namespace IdeaStatiCa.BimApi
{
	/// <summary>
	/// A cross-section to import by name, any name is allowed. 
	/// The correct cross-section is resolved by the user in CCM/Checkbot.
	/// <see cref="IIdeaObject.Name"/> must not be null.
	/// </summary>
	public interface IIdeaCrossSectionByName : IIdeaCrossSection
	{
		/// <summary>
		/// Material of the cross-section.
		/// </summary>
		IIdeaMaterial Material { get; }
	}
}