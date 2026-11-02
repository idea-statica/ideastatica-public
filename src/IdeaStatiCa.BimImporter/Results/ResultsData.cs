using IdeaRS.OpenModel.Result;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApi.Results;
using System;
using System.Collections.Generic;

namespace IdeaStatiCa.BimImporter.Results
{
	public sealed class ResultsData
	{
		public IIdeaObjectWithResults Object { get; }

		public MemberType MemberType { get; }

		public IReadOnlyCollection<IIdeaResult> Results { get; }

		public ResultsData(IIdeaObjectWithResults obj, MemberType memberType, IReadOnlyCollection<IIdeaResult> results)
		{
			Object = obj ?? throw new ArgumentNullException(nameof(obj));
			MemberType = memberType;
			Results = results ?? new List<IIdeaResult>();
		}
	}
}