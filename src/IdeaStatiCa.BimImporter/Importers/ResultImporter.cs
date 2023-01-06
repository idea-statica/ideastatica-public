using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Result;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApi.Results;
using IdeaStatiCa.BimImporter.Common;
using IdeaStatiCa.BimImporter.Results;
using IdeaStatiCa.Plugin;
using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal class ResultImporter : IResultImporter
	{
		private readonly IPluginLogger _logger;

		private readonly DoubleApproximateEqualityComparer _doubleApproximateEqualityComparer
			= new DoubleApproximateEqualityComparer();

		public ResultImporter(IPluginLogger logger)
		{
			_logger = logger;
		}

		public IEnumerable<ResultOnMember> Import(IImportContext ctx, ReferenceElement referenceElement, ResultsData resultsData)
		{
			if (ctx is null)
			{
				throw new ArgumentNullException(nameof(ctx));
			}

			if (referenceElement is null)
			{
				throw new ArgumentNullException(nameof(referenceElement));
			}

			if (resultsData is null)
			{
				throw new ArgumentNullException(nameof(resultsData));
			}

			Member member = new Member
			{
				Id = referenceElement.Id,
				MemberType = resultsData.MemberType
			};

			return resultsData.Results.Select(x => ImportResult(ctx, x)).Select(x =>
			{
				x.Member = member;
				return x;
			});
		}

		private ResultOnMember ImportResult(IImportContext ctx, IIdeaResult result)
		{
			double sectionPositionPrecision = ctx.Configuration.ResultSectionPositionPrecision;

			ResultOnMember resultOnMember = new ResultOnMember
			{
				ResultType = ResultType.InternalForces,
				LocalSystemType = result.CoordinateSystemType
			};

			_doubleApproximateEqualityComparer.Precision = sectionPositionPrecision;
			Dictionary<double, Section> sections = new Dictionary<double, Section>(_doubleApproximateEqualityComparer);

			foreach (IIdeaSection section in result.Sections)
			{
				double position = GetNormalizedPosition(sectionPositionPrecision, section);

				// 0.0 and 1.0 are already normalized at this point
				// so we dont need to include epsilon in the comparison
				if (position < 0.0 || position > 1.0)
				{
					throw new ConstraintException("The position of a section must be within 0 and 1 (including).");
				}

				if (!sections.TryGetValue(position, out Section resultSection))
				{
					resultSection = new Section(position);
					sections.Add(position, resultSection);
				}

				foreach (IIdeaSectionResult res in section.Results)
				{
					var loading = res.Loading;

					if (resultSection.Contains(res.Loading))
					{
						throw new ConstraintException($"Duplicated load case '{loading.Id}' in result section {position}.");
					}

					resultSection.Add(loading, ImportSectionResult(ctx, res));
				}
			}

			// order sections by their position from 0 to 1
			resultOnMember.Results = sections
				.Select(x => x.Value.ResultOnSection)
				.OrderBy(x => x.Position)
				.Cast<ResultBase>()
				.ToList();

			return resultOnMember;
		}

		private double GetNormalizedPosition(double sectionPositionPrecision, IIdeaSection section)
		{
			double position = section.Position;
			double precision = sectionPositionPrecision / 2.0;

			if (position.AlmostEqual(1.0, precision))
			{
				_logger.LogTrace($"Normalizing section position from '{position}' to 1.0");
				position = 1.0;
			}
			else if (position.AlmostEqual(0.0, precision))
			{
				_logger.LogTrace($"Normalizing section position from '{position}' to 0.0");
				position = 0.0;
			}

			return position;
		}

		private SectionResultBase ImportSectionResult(IImportContext ctx, IIdeaSectionResult sectionResult)
		{
			IIdeaLoading loading = sectionResult.Loading;

			if (!(loading is IIdeaLoadCase || loading is IIdeaCombiInput))
			{
				throw new ConstraintException($"{nameof(IIdeaSectionResult.Loading)} property of {nameof(IIdeaSectionResult)} " +
					$"must be instance of {nameof(IIdeaLoadCase)}");
			}

			SectionResultBase result = ConvertResultData(sectionResult.Data);

			result.Loading = new ResultOfLoading()
			{
				LoadingType = loading is IIdeaLoadCase ? LoadingType.LoadCase : LoadingType.Combination,
				Id = ctx.Import(loading).Id
			};

			result.Loading.Items.Add(new ResultOfLoadingItem()
			{
				Coefficient = 1.0
			});

			return result;
		}

		private SectionResultBase ConvertResultData(IIdeaResultData resultData)
		{
			if (!(resultData is InternalForcesData internalForcesData))
			{
				throw new ConstraintException("Currently only results for internal forces are supported.");
			}

			return new ResultOfInternalForces()
			{
				N = internalForcesData.N,
				Qy = internalForcesData.Qy,
				Qz = internalForcesData.Qz,
				Mx = internalForcesData.Mx,
				My = internalForcesData.My,
				Mz = internalForcesData.Mz
			};
		}

		private sealed class Section
		{
			public ResultOnSection ResultOnSection { get; }

			private readonly HashSet<string> _loadCaseIds = new HashSet<string>();
			private readonly List<SectionResultBase> _results = new List<SectionResultBase>();

			public Section(double position)
			{
				ResultOnSection = new ResultOnSection()
				{
					AbsoluteRelative = AbsoluteRelative.Relative,
					Position = position,
					Results = _results
				};
			}

			public bool Contains(IIdeaLoading loading)
				=> _loadCaseIds.Contains(loading.Id);

			public void Add(IIdeaLoading loading, SectionResultBase result)
			{
				_loadCaseIds.Add(loading.Id);
				_results.Add(result);
			}
		}
	}
}