using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.BimApi;

namespace IdeaStatiCa.BimApiLink.BimApi
{
	public class IdeaConnectedMember : AbstractIdeaObject<IIdeaConnectedMember>, IIdeaConnectedMember
	{
		public IdeaConnectedMember(Identifier<IIdeaConnectedMember> identifier) : base(identifier)
		{
		}

		public IdeaConnectedMember(string id)
			: this(new ConnectedMemberIdentifier<IIdeaConnectedMember>(id))
		{

		}

		public IdeaGeometricalType GeometricalType { get; set; }

		public IdeaConnectedMemberType ConnectedMemberType { get; set; }

		public bool IsBearing { get; set; }

		public IdeaForcesIn ForcesIn { get; set; }

		public IdeaBeamSegmentModelType MemberSegmentType { get; set; }

		public IIdeaMember1D IdeaMember { get; set; }

		public bool AutoAddCutByWorkplane { get; set; }

		public bool IsReferenceLineInCenterOfGravity { get; set; }

		public IIdeaPersistenceToken Token
		{
			get => new ConnectedMemberIdentifier<IIdeaConnectedMember>(Identifier.GetId().ToString())
			{
				GeometricalType = GeometricalType,
				ConnectedMemberType = ConnectedMemberType,
				ForcesIn = ForcesIn,
				AutoAddCutByWorkplane = AutoAddCutByWorkplane,
				IsBearing = IsBearing,
				MemberSegmentType = MemberSegmentType,
				IsReferenceLineInCenterOfGravity = IsReferenceLineInCenterOfGravity
			};
		}


	}
}
