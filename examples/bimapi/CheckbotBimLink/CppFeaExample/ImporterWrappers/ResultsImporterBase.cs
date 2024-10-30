using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Results;

namespace ImporterWrappers
{
	public class ResultsImporterBase : IInternalForcesImporter<IIdeaMember1D>
	{
		public ResultsImporterBase()
		{
		}

		public virtual IEnumerable<ResultsData<IIdeaMember1D>> GetResults(IReadOnlyList<IIdeaMember1D> objects)
		{
			throw new NotImplementedException();
		}
	}
}
