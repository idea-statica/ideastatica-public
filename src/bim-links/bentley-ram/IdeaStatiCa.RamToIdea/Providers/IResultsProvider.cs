using RAMDATAACCESSLib;
using System.Collections.Generic;

namespace IdeaStatiCa.RamToIdea.Providers
{
	internal interface IResultsProvider : IDataCache
	{
		void Prefetch(int memberNo);

		//IEnumerable<(Loading, MemberForces)> GetInternalForces(int memberNo);
	}
}
