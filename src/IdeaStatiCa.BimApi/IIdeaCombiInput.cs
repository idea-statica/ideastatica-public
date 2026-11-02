using System.Collections.Generic;

namespace IdeaStatiCa.BimApi
{
	public interface IIdeaCombiInput : IIdeaLoading
	{
		/// <summary>
		/// Type of EC combination
		/// </summary>
		IdeaRS.OpenModel.Loading.TypeOfCombiEC TypeCombiEC { get; }

		/// <summary>
		/// Type of combination
		/// </summary>
		IdeaRS.OpenModel.Loading.TypeCalculationCombiEC TypeCalculationCombi { get; }

		List<IIdeaCombiItem> CombiItems { get; }
	}
}