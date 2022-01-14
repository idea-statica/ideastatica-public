using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApi.Results;
using RAMDATAACCESSLib;
using System.Collections.Generic;

namespace IdeaStatiCa.RamToIdea.Factories
{
	internal interface IResultsFactory
	{
		IEnumerable<IIdeaResult> GetResultsForBeam(IBeam ramBeam, IIdeaMember1D ideaMember);
		IEnumerable<IIdeaResult> GetResultsForColumn(IColumn column);
		IEnumerable<IIdeaResult> GetResultsForVerticalBrace(IVerticalBrace brace);
		IEnumerable<IIdeaResult> GetResultsForHorizontalBrace(IHorizBrace brace);
	}
}