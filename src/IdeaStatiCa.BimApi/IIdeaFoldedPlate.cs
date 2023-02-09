using System.Collections.Generic;

namespace IdeaStatiCa.BimApi
{
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
