using RAMDATAACCESSLib;
using IdeaStatiCa.BimApi.Results;
using System.Collections.Generic;

namespace IdeaStatiCa.RamToIdea.Factories
{
	internal interface IResultsFactory
	{
		//TODO Need to Create a class which fetches results for different member types
		IEnumerable<IIdeaResult> GetResultsForMember(IMember member);
	}
}
