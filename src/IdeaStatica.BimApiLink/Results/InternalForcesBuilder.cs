using IdeaRS.OpenModel.Result;
using IdeaStatica.BimApiLink.Identifiers;
using IdeaStatica.BimApiLink.Results.BimApi;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApi.Results;
using System.Collections;

namespace IdeaStatica.BimApiLink.Results
{
	public sealed class InternalForcesBuilder<T> : BimLinkObject, IEnumerable<ResultsData<T>>
		where T : IIdeaObjectWithResults
	{
		private readonly ResultLocalSystemType _resultLocalSystem;
		private readonly Dictionary<string, IdeaResult> _results = new();
		private readonly HashSet<ResultsData<T>> _resultsData = new();

		public InternalForcesBuilder(ResultLocalSystemType resultLocalSystem)
		{
			_resultLocalSystem = resultLocalSystem;
		}

		public Sections For(T obj, Identifier<IIdeaLoadCase> loadCaseIdentifier) => For(obj, Get(loadCaseIdentifier));

		public Sections For(Identifier<T> objIdentifier, IIdeaLoadCase loadCase) => For(Get(objIdentifier), loadCase);

		public Sections For(T obj, IIdeaLoadCase loadCase)
		{
			if (!_results.TryGetValue(obj.Id, out IdeaResult? result))
			{
				result = new(_resultLocalSystem);
				_results.Add(obj.Id, result);
			}

			_resultsData.Add(new ResultsData<T>(obj, result));
			return new Sections(loadCase, result);
		}

		public IEnumerator<ResultsData<T>> GetEnumerator() => _resultsData.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => _resultsData.GetEnumerator();

		public sealed class Sections
		{
			private readonly IIdeaLoadCase _loadCase;
			private readonly IdeaResult _result;

			private readonly Dictionary<double, IdeaSection> _sections = new();


			internal Sections(IIdeaLoadCase loadCase, IdeaResult result)
			{
				_loadCase = loadCase;
				_result = result;
			}

			public Sections Add(double position, double n, double qy, double qz, double mx, double my, double mz)
			{
				InternalForcesData data = new()
				{
					N = n,
					Qy = qy,
					Qz = qz,
					Mx = mx,
					My = my,
					Mz = mz
				};

				GetSection(position).Add(new IdeaSectionResult(_loadCase, data));

				return this;
			}

			private IdeaSection GetSection(double position)
			{
				if (!_sections.TryGetValue(position, out IdeaSection? section))
				{
					section = new(position);
					_sections.Add(position, section);
					_result.Add(section);
				}

				return section;
			}
		}
	}
}