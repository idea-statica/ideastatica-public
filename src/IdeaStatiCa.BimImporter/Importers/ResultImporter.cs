using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Result;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;
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

		public IEnumerable<ResultOnMember> Import(ReferenceElement referenceElement, IIdeaObjectWithResults obj)
		{
			if (obj is null)
			{
				throw new ArgumentNullException(nameof(obj));
			}

			if (!(obj is IIdeaMember1D || obj is IIdeaElement1D))
			{
				throw new ArgumentException($"Object with results must be an instance of {nameof(IIdeaMember1D)} or {nameof(IIdeaElement1D)}");
			}

			IEnumerable<IIdeaResult> results = obj.GetResults();
			if (results is null)
			{
				_logger.LogDebug($"{nameof(IIdeaObjectWithResults)}.{nameof(IIdeaObjectWithResults.GetResults)} returned nuĺl.");
				yield break;
			}

			Member member = new Member
			{
				Id = referenceElement.Id,
				MemberType = obj is IIdeaMember1D ? MemberType.Member1D : MemberType.Element1D
			};

			foreach (ResultOnMember resultOnMember in results)
			{
				resultOnMember.Member = member;
				yield return resultOnMember;
			}
		}

		private ResultOnMember ImportResult(IIdeaResult result)
		{
			return new ResultOnMember
			{
				LocalSystemType = result.CoordinateSystemType,
				ResultType = result.Type,
				Results = result.Sections.Select(this.ImportMemberSection).ToList()
			};
		}

		private ResultBase ImportMemberSection(IIdeaResultSection memberSection)
		{
			return new ResultOnSection()
			{
				AbsoluteRelative = memberSection.AbsoluteOrRelative ? AbsoluteRelative.Absolute : AbsoluteRelative.Relative,
				Position = memberSection.Position,
				Results = memberSection.Results
			};
		}
	}
}