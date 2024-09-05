namespace IdeaStatiCa.BimApi
{
	public interface IIdeaPinByParameters : IIdeaPin
	{
		/// <summary>
		/// Diameter
		/// </summary>
		double Diameter { get; }

		/// <summary>
		/// Hole Diameter
		/// </summary>
		double HoleDiameter { get; }

		/// <summary>
		/// Has Pin Cap
		/// </summary>
		bool HasPinCap { get; }

		/// <summary>
		/// Pin Cap Diameter
		/// </summary>
		double PinCapDiameter { get; }

		/// <summary>
		/// PinCapThickness
		/// </summary>
		double PinCapThickness { get; }

		/// <summary>
		/// Pin Over lap
		/// </summary>
		double PinOverlap { get; }

		/// <summary>
		/// Material of the pin
		/// </summary>
		IIdeaMaterial Material { get; }
	}
}
