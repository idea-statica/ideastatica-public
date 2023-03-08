using System.Collections.Generic;

namespace IdeaStatiCa.BimApi
{
	/// <summary>
	/// Represent folded plate - connect IIdeaPlate and IIdeaBend in to one piece
	/// </summary>
	public interface IIdeaFoldedPlate : IIdeaObjectConnectable
	{
		/// <summary>
		/// Collection of plates
		/// </summary>
		IEnumerable<IIdeaPlate> Plates { get; }

		/// <summary>
		/// Collection of plates
		/// </summary>
		IEnumerable<IIdeaBend> Bends { get; }
	}


}
