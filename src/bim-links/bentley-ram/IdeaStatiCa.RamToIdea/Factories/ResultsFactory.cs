using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApi.Results;
using IdeaStatiCa.RamToIdea.BimApi;
using IdeaStatiCa.RamToIdea.Providers;
using RAMDATAACCESSLib;
using System.Collections.Generic;
using MathNet.Spatial.Euclidean;
using System.Linq;

namespace IdeaStatiCa.RamToIdea.Factories
{
	internal class ResultsFactory : IResultsFactory
	{
		//private readonly static IIdeaLogger _logger = IdeaDiagnostics.GetLogger("bim.ram.resultsfactory");

		private readonly IObjectFactory _objectFactory;
		private readonly ILoadsProvider _loadsProvider;
		private readonly IForces1 _forces1;
		private readonly IForces2 _forces2;

		public ResultsFactory(IObjectFactory objectFactory, ILoadsProvider loadsProvider, IForces1 forces1, IForces2 forces2)
		{
			_objectFactory = objectFactory;
			_loadsProvider = loadsProvider;
			_forces1 = forces1;
			_forces2 = forces2;
		}

		public IEnumerable<IIdeaResult> GetResultsForBeam(IBeam ramBeam, IIdeaMember1D ideaMember)
		{
			//_logger.LogDebug($"Getting results for member '{beam.lUID}'");

			// Get all load cases
			var loadCases = _loadsProvider.GetLoadCases();

			// Get sections, where results are required
			var locations = GetResultLocations(ideaMember);

			// results for beam - sections - loadcases
			var sections = new List<IIdeaSection>(locations.Count);
			var results = new RamResult
			{
				CoordinateSystemType = IdeaRS.OpenModel.Result.ResultLocalSystemType.Principle,
				Sections = sections,
			};

			foreach (double local in locations)
			{
				var sectionResults = new List<IIdeaSectionResult>(10);
				var section = new RamSection { Position = local, Results = sectionResults, };
				foreach (var loadCase in loadCases)
				{
					var ideaLoadCase = _objectFactory.GetLoadCase(loadCase.lUID);
					if (ramBeam.eFramingType == EFRAMETYPE.MemberIsLateral)
					{
						double pdAxial = 0.0, pdMajMom = 0.0, pdMinMom = 0.0, pdMajShear = 0.0, pdMinShear = 0.0, pdTorsion = 0.0;
						_forces1.GetLatBeamForcesLeftAt(ramBeam.lUID, loadCase.lAnalyzeNo, local, ref pdAxial, ref pdMajMom, ref pdMinMom, ref pdMajShear, ref pdMinShear, ref pdTorsion);
						sectionResults.Add(new RamSectionResult(ideaLoadCase, CreateInternalForces(pdAxial, pdMajShear, pdMinShear, pdTorsion, pdMajMom, pdMinMom)));
					}
					else if (ramBeam.eFramingType == EFRAMETYPE.MemberIsGravity)
					{
						double pdDeadMoment = 0.0, pdDeadShear = 0.0, pdCDMoment = 0.0, pdCDShear = 0.0, pdCLMoment = 0.0, pdCLShear = 0.0, pdPosLiveMoment = 0.0, pdPosLiveShear = 0.0, pdNegLiveMoment = 0.0, pdNegLiveShear = 0.0;

						_forces1.GetGravBeamForcesLeftAt(ramBeam.lUID, local, ref pdDeadMoment, ref pdDeadShear, ref pdCDMoment, ref pdCDShear, ref pdCLMoment, ref pdCLShear, ref pdPosLiveMoment, ref pdPosLiveShear, ref pdNegLiveMoment, ref pdNegLiveShear);

						switch (loadCase.eLoadType)
						{
							case ELoadCaseType.DeadLCa:
								sectionResults.Add(new RamSectionResult(ideaLoadCase, CreateInternalForces(0.0, pdDeadShear, 0.0, 0.0, pdDeadMoment, 0.0)));
								break;

							case ELoadCaseType.LiveLCa:
							case ELoadCaseType.LiveReducibleLCa:
							case ELoadCaseType.LiveRoofLCa:
							case ELoadCaseType.LiveStorageLCa:
							case ELoadCaseType.LiveUnReducibleLCa:
								double liveShear = pdNegLiveShear == 0.0 ? pdPosLiveShear : pdNegLiveShear;
								double liveMoment = pdNegLiveMoment == 0.0 ? pdPosLiveMoment : pdNegLiveMoment;

								sectionResults.Add(new RamSectionResult(ideaLoadCase, CreateInternalForces(0.0, liveShear, 0.0, 0.0, liveMoment, 0.0)));
								break;

							case ELoadCaseType.ConstructionDeadLCa:
								sectionResults.Add(new RamSectionResult(ideaLoadCase, CreateInternalForces(0.0, pdCDShear, 0.0, 0.0, pdCDMoment, 0.0)));
								break;

							case ELoadCaseType.ConstructionLiveLCa:
								sectionResults.Add(new RamSectionResult(ideaLoadCase, CreateInternalForces(0.0, pdCLShear, 0.0, 0.0, pdCLMoment, 0.0)));
								break;

							default:
								sectionResults.Add(new RamSectionResult(ideaLoadCase, CreateInternalForces()));
								break;
						}
					}
				}
			}

			return new IIdeaResult[] { results };
		}

		public IEnumerable<IIdeaResult> GetResultsForColumn(IColumn column)
		{
			//_logger.LogDebug($"Getting results for member '{column.lUID}'");

			// Get all load cases
			var loadCases = _loadsProvider.GetLoadCases();

			// Results for beam - sections - loadcases
			var sections = new List<IIdeaSection>(2);
			var results = new RamResult
			{
				CoordinateSystemType = IdeaRS.OpenModel.Result.ResultLocalSystemType.Principle,
				Sections = sections,
			};

			// Column has only 2 sections - at the begin and at the end of the member
			var sectionResults0 = new List<IIdeaSectionResult>(2);
			var sectionResults1 = new List<IIdeaSectionResult>(2);
			var section0 = new RamSection { Position = 0, Results = sectionResults0, };
			var section1 = new RamSection { Position = 1, Results = sectionResults1, };
			sections.Add(section0);
			sections.Add(section1);

			foreach (var loadCase in loadCases)
			{
				var ideaLoadCase = _objectFactory.GetLoadCase(loadCase.lUID);
				if (column.eFramingType == EFRAMETYPE.MemberIsLateral)
				{
					double pdAxialI = 0.0, pdMajMomI = 0.0, pdMinMomI = 0.0, pdMajShearI = 0.0, pdMinShearI = 0.0, pdTorsionI = 0.0, pdAxialJ = 0.0, pdMajMomJ = 0.0, pdMinMomJ = 0.0, pdMajShearJ = 0.0, pdMinShearJ = 0.0, pdTorsionJ = 0.0;

					_forces2.GetColForcesForLCase(column.lUID, loadCase.lAnalyzeNo, ref pdAxialI, ref pdMajMomI, ref pdMinMomI, ref pdMajShearI, ref pdMinShearI, ref pdTorsionI, ref pdAxialJ, ref pdMajMomJ, ref pdMinMomJ, ref pdMajShearJ, ref pdMinShearJ, ref pdTorsionJ);

					sectionResults0.Add(new RamSectionResult(ideaLoadCase, CreateInternalForces(pdAxialI, pdMajShearI, pdMinShearI, pdTorsionI, pdMajMomI, pdMajMomI)));
					sectionResults1.Add(new RamSectionResult(ideaLoadCase, CreateInternalForces(pdAxialJ, pdMajShearJ, pdMinShearJ, pdTorsionJ, pdMajMomJ, pdMajMomJ)));
				}
				else if (column.eFramingType == EFRAMETYPE.MemberIsGravity)
				{
					double pdDead = 0.0, pdPosLLRed = 0.0, pdPosLLNonRed = 0.0, pdPosLLStorage = 0.0, pdPosLLRoof = 0.0, pdNegLLRed = 0.0, pdNegLLNonRed = 0.0, pdNegLLStorage = 0.0, pdNegLLRoof = 0.0;

					_forces1.GetGrvColForcesForLCase(column.lUID, ref pdDead, ref pdPosLLRed, ref pdPosLLNonRed, ref pdPosLLStorage, ref pdPosLLRoof, ref pdNegLLRed, ref pdPosLLNonRed, ref pdNegLLStorage, ref pdNegLLRoof);

					double nx = 0;
					switch (loadCase.eLoadType)
					{
						case ELoadCaseType.DeadLCa:
							nx = pdDead;
							break;

						case ELoadCaseType.LiveLCa:
						case ELoadCaseType.LiveUnReducibleLCa:
							nx = pdNegLLNonRed == 0.0 ? pdPosLLNonRed : pdNegLLNonRed;
							break;

						case ELoadCaseType.LiveReducibleLCa:
							nx = pdNegLLRed == 0.0 ? pdPosLLRed : pdNegLLRed;
							break;

						case ELoadCaseType.LiveRoofLCa:
							nx = pdNegLLRoof == 0.0 ? pdPosLLRoof : pdNegLLRoof;
							break;

						case ELoadCaseType.LiveStorageLCa:
							nx = pdNegLLStorage;
							break;
					}

					sectionResults0.Add(new RamSectionResult(ideaLoadCase, CreateInternalForces(nx)));
					sectionResults1.Add(new RamSectionResult(ideaLoadCase, CreateInternalForces(nx)));
				}
			}

			return new IIdeaResult[] { results };
		}

		public IEnumerable<IIdeaResult> GetResultsForVerticalBrace(IVerticalBrace brace)
		{
			return GetBraceResults(brace.lUID);
		}

		public IEnumerable<IIdeaResult> GetResultsForHorizontalBrace(IHorizBrace brace)
		{
			return GetBraceResults(brace.lUID);
		}

		private IEnumerable<IIdeaResult> GetBraceResults(int uid)
		{
			//_logger.LogDebug($"Getting results for member '{uid}'");

			// Get all load cases
			var loadCases = _loadsProvider.GetLoadCases();

			// results for beam - sections - loadcases
			var sections = new List<IIdeaSection>(2);
			var results = new RamResult
			{
				CoordinateSystemType = IdeaRS.OpenModel.Result.ResultLocalSystemType.Principle,
				Sections = sections,
			};

			// for column are only 2 sections - at the begin and at the end of the member.
			var sectionResults0 = new List<IIdeaSectionResult>(2);
			var sectionResults1 = new List<IIdeaSectionResult>(2);
			var section0 = new RamSection { Position = 0, Results = sectionResults0, };
			var section1 = new RamSection { Position = 1, Results = sectionResults1, };
			sections.Add(section0);
			sections.Add(section1);

			foreach (var loadCase in loadCases)
			{
				var ideaLoadCase = _objectFactory.GetLoadCase(loadCase.lUID);

				double pdAxial = 0.0, pdMajMomTop = 0.0, pdMinMomTop = 0.0, pdMajShearTop = 0.0, pdMinShearTop = 0.0, pdTorsionTop = 0.0, pdMajMomBot = 0.0, pdMinMomBot = 0.0, pdMajShearBot = 0.0, pdMinShearBot = 0.0, pdTorsionBot = 0.0;
				_forces2.GetLatBraceForces(uid, loadCase.lAnalyzeNo, ref pdAxial, ref pdMajMomTop, ref pdMinMomTop, ref pdMajShearTop, ref pdMinShearTop, ref pdTorsionTop, ref pdMajMomBot, ref pdMinMomBot, ref pdMajShearBot, ref pdMinShearBot, ref pdTorsionBot);

				sectionResults0.Add(new RamSectionResult(ideaLoadCase, CreateInternalForces(pdAxial, pdMajShearBot, pdMinShearBot, pdTorsionBot, pdMajMomBot, pdMajMomBot)));
				sectionResults1.Add(new RamSectionResult(ideaLoadCase, CreateInternalForces(pdAxial, pdMajShearTop, pdMinShearTop, pdTorsionTop, pdMajMomTop, pdMajMomTop)));
			}

			return new IIdeaResult[] { results };
		}

		private static IList<double> GetResultLocations(IIdeaMember1D ideaMember)
		{
			const double MinSectionsDistance = 1e-6;
			const double MaxSectionsDistance = 1;
			var locations = new List<double>();
			if (ideaMember.Elements?.Count > 0)
			{
				var points = new List<IIdeaNode>(ideaMember.Elements.Count * 2);
				double length = 0;

				// collect all nodes from elements
				foreach (var e in ideaMember.Elements)
				{
					var geometry = e.Segment;
					length += (geometry.EndNode.ToMNVector() - geometry.EndNode.ToMNVector()).Length;
					points.Add(geometry.StartNode);
					points.Add(geometry.EndNode);
				}

				// distinct same nodes
				points = points.Distinct().ToList();
				var firstPoint = points[0].ToMNVector();

				// begin position
				locations.Add(0);

				// intermediate nodes
				for (var i = 1; i < points.Count - 1; ++i)
				{
					var point = points[i].ToMNVector();
					var pos = (point - firstPoint).Length;

					// intermediate possitions means connected beam - we add position a bit before and a bit after section
					locations.Add(pos - MinSectionsDistance / 2);
					locations.Add(pos + MinSectionsDistance / 2);
				}

				// position at the end of the member
				locations.Add(length);

				// add some sections, if distance between locations is too big
				var count = locations.Count;
				for (var i = count - 2; i >= 0; --i)
				{
					var dist = locations[i] - locations[i + 1];
					if (dist > MaxSectionsDistance)
					{
						var n = (int)(dist / MaxSectionsDistance);
						var d = dist / n;
						for (int j = n; j > 0; --j)
						{
							locations.Insert(i, locations[i] + d * j);
						}
					}
				}

				for (var i = 0; i < locations.Count; ++i)
				{
					locations[i] /= length;
				}
			}

			return locations;
		}

		private static IIdeaResultData CreateInternalForces(double nx = 0, double vy = 0, double vz = 0, double mx = 0, double my = 0, double mz = 0)
		{
			return new InternalForcesData
			{
				N = nx,
				Qy = vy,
				Qz = vz,
				Mx = mx,
				My = my,
				Mz = mz,
			};
		}
	}

	internal static class IdeaGeometryExtensions
	{
		public static Vector3D ToMNVector(this IIdeaNode node)
		{
			return node.Vector.ToMNVector();
		}

		public static Vector3D ToMNVector(this IdeaVector3D v)
		{
			return new Vector3D(v.X, v.Y, v.Z);
		}
	}
}