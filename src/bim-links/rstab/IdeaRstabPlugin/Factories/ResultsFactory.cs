using Dlubal.RSTAB8;
using IdeaRS.OpenModel.Result;
using IdeaRstabPlugin.BimApi;
using IdeaRstabPlugin.Factories;
using IdeaRstabPlugin.Providers;
using IdeaRstabPlugin.Utilities;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApi.Results;
using MathNet.Numerics;
using System.Collections.Generic;
using System.Linq;

namespace IdeaRstabPlugin.Factories
{
	internal class ResultsFactory : IResultsFactory
	{
		public const double ResultsPrecision = 1e-5;

		private static readonly DoubleApproximateEquals _doubleApproximateEquals = new DoubleApproximateEquals();

		private sealed class DoubleApproximateEquals : IEqualityComparer<double>
		{
			public bool Equals(double x, double y)
			{
				return x.AlmostEqual(y, ResultsPrecision);
			}

			public int GetHashCode(double x)
			{
				return x.Round(5).GetHashCode();
			}
		}

		private readonly IObjectFactory _objectFactory;
		private readonly IResultsProvider _resultsProvider;
		private readonly IImportSession _importSession;

		public ResultsFactory(IObjectFactory objectFactory, IResultsProvider resultsProvider, IImportSession importSession)
		{
			_objectFactory = objectFactory;
			_resultsProvider = resultsProvider;
			_importSession = importSession;
		}

		public IEnumerable<IIdeaResult> GetResultsForMember(Dlubal.RSTAB8.Member member)
		{
			return new List<IIdeaResult>() { GetInternalForces(member) };
		}

		private IIdeaResult GetInternalForces(Dlubal.RSTAB8.Member member)
		{
			PairStorage<double, RstabSectionResult> results
				= new PairStorage<double, RstabSectionResult>(_doubleApproximateEquals, EqualityComparer<RstabSectionResult>.Default);
			PairStorage<double, IIdeaLoading> loads
				= new PairStorage<double, IIdeaLoading>(_doubleApproximateEquals, EqualityComparer<IIdeaLoading>.Default);

			foreach (RstabSectionResult sectionResult in GetSectionResults(member))
			{
				if (!loads.Contains(sectionResult.Position, sectionResult.Loading))
				{
					results.Add(sectionResult.Position, sectionResult);
					loads.Add(sectionResult.Position, sectionResult.Loading);
				}
			}

			InterpolateMissingResults(results, loads);

			RstabResult result = new RstabResult
			{
				CoordinateSystemType = ResultLocalSystemType.Local,
				Sections = results
					.EnumerateByLeft()
					.Select(x => new RstabSection()
					{
						Position = x.Key,
						Results = x
					})
					.Cast<IIdeaSection>()
			};

			return result;
		}

		private static void InterpolateMissingResults(
			PairStorage<double, RstabSectionResult> results,
			PairStorage<double, IIdeaLoading> loads)
		{
			List<double> allPositions = new List<double>(results.GetLefts());
			allPositions.Sort();

			foreach (IIdeaLoading load in loads.GetRights())
			{
				if (loads.GetLefts(load).Count() == allPositions.Count)
				{
					continue;
				}

				List<double> positions = new List<double>(loads.GetLefts(load));
				positions.Sort();

				int i = 1, j = 1;
				while (i < allPositions.Count - 1 && j < positions.Count - 1)
				{
					double pos = allPositions[i];

					if (pos == positions[j])
					{
						i++;
						j++;
						continue;
					}

					RstabSectionResult a = results
						.GetRights(positions[j - 1])
						.FirstOrDefault(x => x.Loading == load);

					RstabSectionResult b = results
						.GetRights(positions[j])
						.FirstOrDefault(x => x.Loading == load);

					if (a != null && b != null)
					{
						results.Add(pos, RstabSectionResult.Interpolate(a, b, pos));
					}

					i++;
				}
			}
		}

		private IEnumerable<RstabSectionResult> GetSectionResults(Dlubal.RSTAB8.Member member)
		{
			bool isCSDownwards = !_importSession.IsGCSOrientedUpwards;

			foreach ((Dlubal.RSTAB8.Loading loading, MemberForces memberForces) in _resultsProvider.GetInternalForces(member.No))
			{
				double position = memberForces.Location / member.Length;

				if (memberForces.Flag == ResultsFlag.LeftSideFlag)
				{
					position -= ResultsPrecision;
				}
				else if (memberForces.Flag == ResultsFlag.RightSideFlag)
				{
					position += ResultsPrecision;
				}
				else if (memberForces.Flag != ResultsFlag.StandardValueFlag)
				{
					continue;
				}

				if (position.IsLarger(1, ResultsPrecision) || position.IsSmaller(0, ResultsPrecision))
				{
					continue;
				}

				yield return new RstabSectionResult(_objectFactory, member, memberForces, loading, isCSDownwards, position);
			}
		}
	}
}