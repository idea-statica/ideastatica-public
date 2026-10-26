using Dlubal.RSTAB8;
using System.Collections.Generic;

namespace IdeaRstabPlugin.Providers
{
	internal interface IResultsProvider: IDataCache
	{
		void Prefetch(int memberNo);

		IEnumerable<(Loading, MemberForces)> GetInternalForces(int memberNo);
	}
}