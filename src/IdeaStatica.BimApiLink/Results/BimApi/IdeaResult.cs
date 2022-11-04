using IdeaRS.OpenModel.Result;
using IdeaStatiCa.BimApi.Results;
using System.Collections.Generic;

namespace IdeaStatica.BimApiLink.Results.BimApi
{
	public class IdeaResult : IIdeaResult
	{
		public ResultLocalSystemType CoordinateSystemType { get; }

		public IEnumerable<IIdeaSection> Sections => _sections;

		private readonly List<IIdeaSection> _sections = new List<IIdeaSection>();

		public IdeaResult(ResultLocalSystemType coordinateSystemType)
		{
			CoordinateSystemType = coordinateSystemType;
		}

		public void Add(IIdeaSection section)
		{
			_sections.Add(section);
		}
	}
}