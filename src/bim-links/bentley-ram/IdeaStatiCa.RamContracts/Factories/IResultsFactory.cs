using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApi.Results;
using RAMDATAACCESSLib;
using System.Collections.Generic;

namespace IdeaStatiCa.RamToIdea.Factories
{
	public interface IResultsFactory
	{
		IEnumerable<IIdeaResult> GetResultsForBeam(IBeam ramBeam, IIdeaMember1D ideaMember);
		IEnumerable<IIdeaResult> GetResultsForColumn(IColumn column, IIdeaMember1D ideaMember);
		IEnumerable<IIdeaResult> GetResultsForVerticalBrace(IVerticalBrace brace, IIdeaMember1D ideaMember);
		IEnumerable<IIdeaResult> GetResultsForHorizontalBrace(IHorizBrace brace, IIdeaMember1D ideaMember);
	}
}