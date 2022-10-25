using IdeaRS.OpenModel.Result;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.Results;
using Nito.Disposables.Internals;

namespace IdeaStatica.BimApiLink.Results
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
			return _internalForcesImporter.GetResults(objects.OfType<T>().ToList())
				.Select(Convert)
				.WhereNotNull();
		}

		private ResultsData? Convert(ResultsData<T> resultsData)
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

		private static MemberType? GetMemberType(T obj) => obj switch
		{
			IIdeaMember1D => MemberType.Member1D,
			IIdeaElement1D => MemberType.Element1D,
			_ => null,
		};
	}
}