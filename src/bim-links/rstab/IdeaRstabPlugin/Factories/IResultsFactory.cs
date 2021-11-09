using Dlubal.RSTAB8;
using IdeaStatiCa.BimApi.Results;
using System.Collections.Generic;

namespace IdeaRstabPlugin.Factories
{
	internal interface IResultsFactory
	{
		IEnumerable<IIdeaResult> GetResultsForMember(Member member);
	}
}