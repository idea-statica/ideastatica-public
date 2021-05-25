using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Result;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApi.Results;
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
			return new ResultOnMember
			{
				LocalSystemType = result.CoordinateSystemType,
				ResultType = result.Type,
				Results = result.Sections.Select(x => ImportMemberSection(ctx, x)).ToList()
			};
		}

		private ResultBase ImportMemberSection(IImportContext ctx, IIdeaSection memberSection)
		{
			double position = memberSection.Position;

			if (position.AlmostEqual(1.0, Constants.Precision))
			{
				position = 1.0;
			}
			else if (position.AlmostEqual(0.0, Constants.Precision))
			{
				position = 0.0;
			}

			if (position < 0.0 || position > 1.0)
			{
				throw new ConstraintException("IIdeaSection.Position must be in a open interval (0;1).");
			}

			return new ResultOnSection()
			{
				AbsoluteRelative = memberSection.AbsoluteOrRelative ? AbsoluteRelative.Absolute : AbsoluteRelative.Relative,
				Position = memberSection.Position,
				Results = memberSection.Results.Select(x => ProcessSectionResult(ctx, x)).ToList()
			};
		}

		private SectionResultBase ProcessSectionResult(IImportContext ctx, IIdeaSectionResult sectionResult)
		{
			IIdeaLoading loading = sectionResult.Loading;

			if (!(loading is IIdeaLoadCase || loading is IIdeaCombiInput))
			{
				throw new ConstraintException($"IIdeaSectionResult.Loading must be instance of {nameof(IIdeaLoadCase)}");
			}

			SectionResultBase result = sectionResult.Result;

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
	}
}