using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.BimApiLink.Importers;

namespace BimApiLinkCadExample.Importers
{
	/// <summary>
	/// A connected member is a member as one connection sees it. Its identifier carries the member's CAD id as text.
	/// </summary>
	internal class ConnectedMemberImporter : ImporterConnectedMemberIdentifier<IIdeaConnectedMember>
	{
		/// <summary>
		/// Creates the connected member for the member in the identifier. The settings below are the same for every
		/// member here; a real link may read them from the CAD model.
		/// </summary>
		public override IIdeaConnectedMember Create(ConnectedMemberIdentifier<IIdeaConnectedMember> id)
		{
			string memberId = id.IdeaMember.GetId().ToString();

			return new IdeaConnectedMember(memberId)
			{
				IdeaMember = Get<IIdeaMember1D>(int.Parse(memberId)),
				ConnectedMemberType = id.ConnectedMemberType,
				GeometricalType = IdeaGeometricalType.Ended,
				ForcesIn = IdeaForcesIn.Position,
				MemberSegmentType = IdeaBeamSegmentModelType.LoadedInXYZ,
				IsReferenceLineInCenterOfGravity = true,
				AutoAddCutByWorkplane = true,
				IsBearing = false,
			};
		}
	}
}
