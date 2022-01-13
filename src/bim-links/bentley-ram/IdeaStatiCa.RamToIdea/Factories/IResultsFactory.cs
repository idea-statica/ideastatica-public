using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApi.Results;
using RAMDATAACCESSLib;
using System.Collections.Generic;

namespace IdeaStatiCa.RamToIdea.Factories
{
	internal interface IResultsFactory
	{
		IEnumerable<IIdeaResult> GetBeamResults(IBeam ramBeam, IIdeaMember1D ideaMember);
		IEnumerable<IIdeaResult> GetColumnResults(IColumn column);
		IEnumerable<IIdeaResult> GetVerticalBraceResults(IVerticalBrace brace);
		IEnumerable<IIdeaResult> GetHorizontalBraceResults(IHorizBrace brace);
	}
}