using IdeaRS.OpenModel.Result;
using IdeaStatiCa.BimApi.Results;

namespace IdeaStatica.BimApiLink.Results.BimApi
{
	public class IdeaResult : IIdeaResult
	{
		public ResultLocalSystemType CoordinateSystemType { get; }

		public IEnumerable<IIdeaSection> Sections => _sections;

		private readonly List<IIdeaSection> _sections = new();

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