namespace IdeaStatiCa.BimApi
{
	public interface IIdeaConcreteBlock : IIdeaObject
	{
		/// <summary>
		/// Lenght
		/// </summary>
		double Lenght { get; }

		/// <summary>
		/// Width
		/// </summary>
		double Width { get; }

		/// <summary>
		/// Height
		/// </summary>
		double Height { get; }

		/// <summary>
		/// Material of the concrete block.
		/// </summary>
		IIdeaMaterial Material { get; }
	}
}
