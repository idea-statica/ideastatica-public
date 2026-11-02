using IdeaRS.OpenModel.Result;
using IdeaStatiCa.BimApi;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.BimImporter.Results
{
	public class DefaultResultsProvider : IBimResultsProvider
	{
		public IEnumerable<ResultsData> GetResults(IEnumerable<IIdeaObjectWithResults> objects)
		{
			foreach (IIdeaObjectWithResults obj in objects)
			{
				IEnumerable<BimApi.Results.IIdeaResult> results = obj.GetResults();
				if (results is null)
				{
					continue;
				}

				MemberType memberType;

				if (obj is IIdeaMember1D)
				{
					memberType = MemberType.Member1D;
				}
				else if (obj is IIdeaElement1D)
				{
					memberType = MemberType.Element1D;
				}
				else
				{
					continue;
				}

				yield return new ResultsData(obj, memberType, results.ToList());
			}
		}
	}
}