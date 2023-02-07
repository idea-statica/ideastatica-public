using IdeaStatiCa.BimApi;

namespace IdeaStatica.BimApiLink.Identifiers
{
	public class ConnectedMemberIdentidier<T> : StringIdentifier<T>
		where T : IIdeaObject
	{
		public IdeaGeometricalType GeometricalType { get; set; }

		public IdeaConnectedMemberType ConnectedMemberType { get; set; }

		public bool IsBearing { get; set; }

		public IdeaForcesIn ForcesIn { get; set; }

		public IdeaBeamSegmentModelType MemberSegmentType { get; set; }

		public bool AutoAddCutByWorkplane { get; set; }

		public Identifier<IIdeaMember1D> IdeaMember { get; set; }

		public ConnectedMemberIdentidier(string id)
			: base(id)
		{
			IdeaMember = new StringIdentifier<IIdeaMember1D>(id);
		}
	}
}