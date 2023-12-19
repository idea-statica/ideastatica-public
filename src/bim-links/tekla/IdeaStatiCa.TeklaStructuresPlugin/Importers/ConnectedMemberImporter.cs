using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.BimApiLink.Importers;

namespace IdeaStatiCa.TeklaStructuresPlugin.Importers
{
	internal class ConnectedMemberImporter : ImporterConnectedMemberIdentifier<IIdeaConnectedMember>
	{
		protected ModelClient Model { get; }

		public ConnectedMemberImporter(ModelClient model)
			: base()
		{
			Model = model;
		}

		public override IIdeaConnectedMember Create(ConnectedMemberIdentifier<IIdeaConnectedMember> id)
		{
			var ideaMember = GetMaybe<IIdeaMember1D>(id.IdeaMember);
			if (ideaMember == null)
			{
				return null;
			}

			//For specific link value skip id value
			var connectedMember = new IdeaConnectedMember(id.IdeaMember.GetId().ToString())
			{
				IsReferenceLineInCenterOfGravity = true,
				AutoAddCutByWorkplane = true,
				IdeaMember = ideaMember,
				ConnectedMemberType = id.ConnectedMemberType,
				ForcesIn = IdeaForcesIn.Position,
				GeometricalType = IdeaGeometricalType.Ended,
				IsBearing = false,
				Name = "",
				MemberSegmentType = IdeaBeamSegmentModelType.LoadedInXYZ,
			};
			return connectedMember;
		}
	}
}
