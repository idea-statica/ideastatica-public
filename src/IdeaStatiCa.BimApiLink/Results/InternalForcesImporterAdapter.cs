using IdeaRS.OpenModel.Result;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.Results;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.BimApiLink.Results
{
	internal class InternalForcesImporterAdapter<T> : IBimResultsProvider
		where T : IIdeaObjectWithResults
	{
		private readonly IInternalForcesImporter<T> _internalForcesImporter;

		public InternalForcesImporterAdapter(IInternalForcesImporter<T> internalForcesImporter)
		{
			_internalForcesImporter = internalForcesImporter;
		}

		public IEnumerable<ResultsData> GetResults(IEnumerable<IIdeaObjectWithResults> objects)
		{
			return _internalForcesImporter.GetResults(objects.OfType<T>().ToArray())
				.Select(Convert)
				.Where(x => x != null);
		}

		private ResultsData Convert(ResultsData<T> resultsData)
		{
			MemberType? memberType = GetMemberType(resultsData.Object);
			if (memberType is null)
			{
				return null;
			}

			return new ResultsData(
				resultsData.Object,
				memberType.Value,
				new[] { resultsData.Results });
		}

		private static MemberType? GetMemberType(T obj)
		{
			switch (obj)
			{
				case IIdeaMember1D _: return MemberType.Member1D;
				case IIdeaElement1D _: return MemberType.Element1D;
				default: return null;
			}
		}
	}
}