using IdeaRS.OpenModel.Connection;

namespace IdeaStatiCa.BimApi
{
	/// <summary>
	/// Represent cut operation
	/// </summary>
	public interface IIdeaCut : IIdeaPersistentObject
	{
		/// <summary>
		/// Modified Object
		/// </summary>
		IIdeaObject ModifiedObject { get; }

		/// <summary>
		/// Modified Object
		/// </summary>
		IIdeaObject CuttingObject { get; }

		/// <summary>
		/// Offset
		/// </summary>
		double Offset { get; }

		/// <summary>
		/// Weld of cut (web weld when separate flange weld is defined)
		/// </summary>
		IIdeaWeld Weld { get; }

		/// <summary>
		/// Flange weld of cut. Null means same as Weld or no separate flange weld.
		/// </summary>
		IIdeaWeld FlangesWeld { get; }

		/// <summary>
		/// Cut Method
		/// </summary>
		CutMethod CutMethod { get; }

		/// <summary>
		/// Cut Orientation
		/// </summary>
		CutOrientation CutOrientation { get; }


		/// <summary>
		/// Distance Comparison
		/// </summary>
		DistanceComparison DistanceComparison { get; }

		/// <summary>
		/// Cut part
		/// </summary>
		CutPart CutPart { get; }

		/// <summary>
		/// Extend before cut - for cuts where user can decide if modified beam will be extended or not
		/// </summary>
		bool ExtendBeforeCut { get; }
	}
}
