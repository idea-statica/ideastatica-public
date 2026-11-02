namespace IdeaStatiCa.BimApi
{
	/// <summary>
	/// A cross-section of an <see cref="IIdeaElement1D"/>.
	/// </summary>
	public interface IIdeaCrossSection : IIdeaObject
	{
		/// <summary>
		/// Rotation of the cross-section.
		/// </summary>
		double Rotation { get; }

		/// <summary>
		/// Specifies that the cross-section is in its principal axis.
		/// </summary>
		bool IsInPrincipal { get; }
	}
}