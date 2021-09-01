using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Result;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApi.Results;
using IdeaStatiCa.BimImporter.Common;
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

		public IEnumerable<ResultOnMember> Import(IImportContext ctx, ReferenceElement referenceElement, IIdeaObjectWithResults obj)
		{
			if (ctx is null)
			{
				throw new ArgumentNullException(nameof(ctx));
			}

			if (referenceElement is null)
			{
				throw new ArgumentNullException(nameof(referenceElement));
			}

			if (obj is null)
			{
				throw new ArgumentNullException(nameof(obj));
			}

			if (!(obj is IIdeaMember1D || obj is IIdeaElement1D))
			{
				throw new ConstraintException($"Object with results must be an instance of {nameof(IIdeaMember1D)}" +
					$" or {nameof(IIdeaElement1D)}");
			}

			IEnumerable<IIdeaResult> results = obj.GetResults();
			if (results is null)
			{
				_logger.LogDebug($"{nameof(IIdeaObjectWithResults)}.{nameof(IIdeaObjectWithResults.GetResults)} returned null.");
				return Enumerable.Empty<ResultOnMember>();
			}

			Member member = new Member
			{
				Id = referenceElement.Id,
				MemberType = obj is IIdeaMember1D ? MemberType.Member1D : MemberType.Element1D
			};

			return results.Select(x => ImportResult(ctx, x)).Select(x =>
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
			HashSet<double> importedPositions = new HashSet<double>(_doubleApproximateEqualityComparer);
			List<ResultOnSection> results = new List<ResultOnSection>();

			foreach (IIdeaSection section in result.Sections)
			{
				double position = section.Position;

				if (position.AlmostEqual(1.0, sectionPositionPrecision))
				{
					_logger.LogTrace($"Normalizing section position from '{position}' to 1.0");
					position = 1.0;
				}
				else if (position.AlmostEqual(0.0, sectionPositionPrecision))
				{
					_logger.LogTrace($"Normalizing section position from '{position}' to 0.0");
					position = 0.0;
				}

				// 0.0 and 1.0 are already normalized at this point
				// so we dont need to include epsilon in the comparison
				if (position < 0.0 || position > 1.0)
				{
					throw new ConstraintException("The position of a section must be within 0 and 1 (including).");
				}

				if (!importedPositions.Add(section.Position))
				{
					throw new ConstraintException($"Result section on the position '{position}' already exists.");
				}

				ResultOnSection resultOnSection = new ResultOnSection()
				{
					AbsoluteRelative = AbsoluteRelative.Absolute,
					Position = position,
					Results = section.Results.Select(x => ImportSectionResult(ctx, x)).ToList()
				};

				results.Add(resultOnSection);
			}

			// order sections by their position from 0 to 1
			resultOnMember.Results = results
				.OrderBy(x => x.Position)
				.Cast<ResultBase>()
				.ToList();

			return resultOnMember;
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
	}
}