using IdeaRS.OpenModel.Result;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApi.Results;
using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.BimApiLink.Results.BimApi;
using System.Collections;
using System.Collections.Generic;

namespace IdeaStatiCa.BimApiLink.Results
{
	public sealed class InternalForcesBuilder<T> : BimLinkObject, IEnumerable<ResultsData<T>>
		where T : IIdeaObjectWithResults
	{
		private readonly ResultLocalSystemType _resultLocalSystem;
		private readonly List<ResultsData<T>> _resultsData = new List<ResultsData<T>>();
		private readonly Dictionary<(string Obj, string LoadCase), Sections> _sections = new Dictionary<(string, string), Sections>();

		public InternalForcesBuilder(ResultLocalSystemType resultLocalSystem)
		{
			_resultLocalSystem = resultLocalSystem;
		}

		public Sections For(T obj, Identifier<IIdeaLoadCase> loadCaseIdentifier)
			=> For(obj, Get(loadCaseIdentifier));

		public Sections For(Identifier<T> objIdentifier, IIdeaLoadCase loadCase)
			=> For(Get(objIdentifier), loadCase);

		public Sections For(T obj, IIdeaLoadCase loadCase)
		{
			if (!_sections.TryGetValue((obj.Id, loadCase.Id), out Sections sections))
			{
				IdeaResult result = new IdeaResult(_resultLocalSystem);
				_resultsData.Add(new ResultsData<T>(obj, result));

				sections = new Sections(loadCase, result);
			}

			return sections;
		}

		public IEnumerator<ResultsData<T>> GetEnumerator() => _resultsData.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => _resultsData.GetEnumerator();

		public sealed class Sections
		{
			private readonly IIdeaLoadCase _loadCase;
			private readonly IdeaResult _result;

			private readonly Dictionary<double, IdeaSection> _sections = new Dictionary<double, IdeaSection>();

			internal Sections(IIdeaLoadCase loadCase, IdeaResult result)
			{
				_loadCase = loadCase;
				_result = result;
			}

			public Sections Add(double position, double n, double qy, double qz, double mx, double my, double mz)
			{
				InternalForcesData data = new InternalForcesData()
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
				if (!_sections.TryGetValue(position, out IdeaSection section))
				{
					section = new IdeaSection(position);
					_sections.Add(position, section);
					_result.Add(section);
				}

				return section;
			}
		}
	}
}