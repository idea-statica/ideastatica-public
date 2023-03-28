using Castle.MicroKernel.SubSystems.Conversion;
using IdeaStatiCa.BimApi;

namespace IdeaStatiCa.BimApiLink.Identifiers
{
	public class ConnectedMemberIdentifier<T> : ImmutableIdentifier<T>
		where T : IIdeaObject
	{
		public IdeaGeometricalType GeometricalType { get; set; }

		public IdeaConnectedMemberType ConnectedMemberType { get; set; }

		public bool IsBearing { get; set; }

		public IdeaForcesIn ForcesIn { get; set; }

		public IdeaBeamSegmentModelType MemberSegmentType { get; set; }

		public bool AutoAddCutByWorkplane { get; set; }

		public bool IsReferenceLineInCenterOfGravity { get; set; }

		public Identifier<IIdeaMember1D> IdeaMember { get; set; }

		public override string GetStringId() => $"{typeof(T).FullName}-{IdeaMember?.GetId()}";

		public override object GetId() => IdeaMember?.GetId();

		public ConnectedMemberIdentifier()
			: base("")
		{
		}

		public ConnectedMemberIdentifier(string id)
			: base(id)
		{
			IdeaMember = new StringIdentifier<IIdeaMember1D>(id);
		}

		public ConnectedMemberIdentifier(Identifier<IIdeaMember1D> id)
			: base(id.GetStringId())
		{
			IdeaMember = id;
		}
	}
}