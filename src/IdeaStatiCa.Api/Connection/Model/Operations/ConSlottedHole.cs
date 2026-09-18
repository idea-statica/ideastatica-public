#nullable enable annotations

namespace IdeaStatiCa.Api.Connection.Model.Operations
{
	/// <summary>
	/// Slotted hole in one plate — used per fastener position or as a grid-level default.
	/// </summary>
	public class ConSlottedHole
	{
		/// <summary>Slot orientation [radians].</summary>
		public double Angle { get; set; }

		/// <summary>Slot length factor relative to the bolt diameter (1.0 = no slot).</summary>
		public double SizeFactor { get; set; } = 1.0;
	}
}
