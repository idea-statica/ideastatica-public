using IdeaRS.OpenModel.Model;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApi.Results;
using System.Collections.Generic;

namespace IdeaStatiCa.BimImporter.ImportedObjects
{
	public class ConnectedMember : IIdeaConnectedMember
	{
		public ConnectedMember(IIdeaMember1D member)
		{
			IdeaMember = member;
		}
		public IIdeaMember1D IdeaMember { get; }

		public IdeaGeometricalType GeometricalType { get; set; } = IdeaGeometricalType.Ended;

		public IdeaConnectedMemberType ConnectedMemberType { get; set; }

		public bool IsBearing { get; set; }

		public IdeaForcesIn ForcesIn { get; set; }

		public IdeaBeamSegmentModelType MemberSegmentType { get; set; }

		public Member1DType Type => IdeaMember.Type;

		public List<IIdeaElement1D> Elements => IdeaMember.Elements;

		public IIdeaPersistenceToken Token => IdeaMember.Token;

		public string Id => $"CM-{IdeaMember.Id}";

		public string Name => IdeaMember.Name;

		public bool AutoAddCutByWorkplane { get; set; }

		public IEnumerable<IIdeaResult> GetResults()
		{
			return IdeaMember.GetResults();
		}
	}
}