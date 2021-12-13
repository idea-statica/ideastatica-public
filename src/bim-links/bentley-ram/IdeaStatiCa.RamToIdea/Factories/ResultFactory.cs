using RAMDATAACCESSLib;
using IdeaRS.OpenModel.Result;
using IdeaStatiCa.RamToIdea.BimApi;
using IdeaStatiCa.RamToIdea.Providers;
using IdeaStatiCa.RamToIdea.Utilities;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApi.Results;
using MathNet.Numerics;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.RamToIdea.Factories
{
	internal class ResultsFactory : IResultsFactory
	{
		//private static readonly DoubleApproximateEquals _doubleApproximateEquals = new DoubleApproximateEquals();

		//private class DoubleApproximateEquals : IEqualityComparer<double>
		//{
		//	public bool Equals(double x, double y)
		//	{
		//		return x.AlmostEqual(y, 1e-6);
		//	}

		//	public int GetHashCode(double x)
		//	{
		//		return x.Round(6).GetHashCode();
		//	}
		//}

		private readonly IObjectFactory _objectFactory;
		private readonly IResultsProvider _resultsProvider;
		private readonly IImportSession _importSession;

		public ResultsFactory(IObjectFactory objectFactory, IResultsProvider resultsProvider, IImportSession importSession)
		{
			_objectFactory = objectFactory;
			_resultsProvider = resultsProvider;
			_importSession = importSession;
		}

		//public IEnumerable<IIdeaResult> GetResultsForMember(Dlubal.RSTAB8.Member member)
		//{
		//	return new List<IIdeaResult>() { GetInternalForces(member) };
		//}

		//private IIdeaResult GetInternalForces(Dlubal.RSTAB8.Member member)
		//{
		//	PairStorage<double, RstabSectionResult> results
		//		= new PairStorage<double, RstabSectionResult>(_doubleApproximateEquals, EqualityComparer<RstabSectionResult>.Default);
		//	PairStorage<double, IIdeaLoading> loads
		//		= new PairStorage<double, IIdeaLoading>(_doubleApproximateEquals, EqualityComparer<IIdeaLoading>.Default);

		//	foreach (RstabSectionResult sectionResult in GetSectionResults(member))
		//	{
		//		if (loads.Contains(sectionResult.Position, sectionResult.Loading))
		//		{
		//			RstabSectionResult existingResult = results
		//				.GetRights(sectionResult.Position)
		//				.Where(x => x.Loading == sectionResult.Loading)
		//				.First();

		//			existingResult.Add(sectionResult);
		//		}
		//		else
		//		{
		//			results.Add(sectionResult.Position, sectionResult);
		//			loads.Add(sectionResult.Position, sectionResult.Loading);
		//		}
		//	}

		//	InterpolateMissingResults(results, loads);

		//	RstabResult result = new RstabResult
		//	{
		//		CoordinateSystemType = ResultLocalSystemType.Principle,
		//		Sections = results
		//			.EnumerateByLeft()
		//			.Select(x => new RstabSection()
		//			{
		//				Position = x.Key,
		//				Results = x
		//			})
		//			.Cast<IIdeaSection>()
		//	};

		//	return result;
		//}

		//private static void InterpolateMissingResults(
		//	PairStorage<double, RstabSectionResult> results,
		//	PairStorage<double, IIdeaLoading> loads)
		//{
		//	List<double> allPositions = new List<double>(results.GetLefts());
		//	allPositions.Sort();

		//	foreach (IIdeaLoading load in loads.GetRights())
		//	{
		//		if (loads.GetLefts(load).Count() == allPositions.Count())
		//		{
		//			continue;
		//		}

		//		List<double> positions = new List<double>(loads.GetLefts(load));
		//		positions.Sort();

		//		int i = 1, j = 1;
		//		while (i < allPositions.Count && j < positions.Count)
		//		{
		//			double pos = allPositions[i];

		//			if (pos == positions[j])
		//			{
		//				i++;
		//				j++;
		//				continue;
		//			}

		//			RstabSectionResult a = results
		//				.GetRights(positions[j - 1])
		//				.Where(x => x.Loading == load)
		//				.FirstOrDefault();

		//			double bPos = 0.0;

		//			if (positions[j] > pos)
		//			{
		//				bPos = positions[j];
		//			}
		//			else if (j < positions.Count - 1)
		//			{
		//				bPos = positions[j + 1];
		//			}
		//			else
		//			{
		//				break;
		//			}

		//			RstabSectionResult b = results
		//				.GetRights(bPos)
		//				.Where(x => x.Loading == load)
		//				.FirstOrDefault();

		//			if (a != null && b != null)
		//			{
		//				results.Add(pos, RstabSectionResult.Interpolate(a, b, pos));
		//			}

		//			i++;
		//		}
		//	}
		//}

		//private IEnumerable<RstabSectionResult> GetSectionResults(Dlubal.RSTAB8.Member member)
		//{
		//	bool isCSDownwards = !_importSession.IsGCSOrientedUpwards;

		//	foreach ((Dlubal.RSTAB8.Loading loading, MemberForces memberForces) in _resultsProvider.GetInternalForces(member.No))
		//	{
		//		if (memberForces.Flag == ResultsFlag.LeftSideFlag)
		//		{
		//			continue;
		//		}
		//		else if (memberForces.Flag == ResultsFlag.RightSideFlag)
		//		{
		//			continue;
		//		}

		//		yield return new RamSectionResult(_objectFactory, member, memberForces, loading, isCSDownwards);
		//	}
		//}
		public IEnumerable<IIdeaResult> GetResultsForMember(IMember member)
		{
			throw new System.NotImplementedException();
		}
	}
}
