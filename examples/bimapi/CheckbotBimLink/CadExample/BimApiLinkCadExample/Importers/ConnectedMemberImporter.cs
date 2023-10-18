using BimApiLinkCadExample.CadExampleApi;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.BimApiLink.Importers;

namespace BimApiLinkCadExample.Importers
{
	internal class ConnectedMemberImporter : ImporterConnectedMemberIdentifier<IIdeaConnectedMember>
	{
		protected ICadGeometryApi Model { get; }

		public ConnectedMemberImporter(ICadGeometryApi model)
			: base()
		{
			Model = model;
		}

		public override IIdeaConnectedMember Create(ConnectedMemberIdentifier<IIdeaConnectedMember> id)
		{
			IntIdentifier<IIdeaMember1D> intIdentifier = new IntIdentifier<IIdeaMember1D>(int.Parse(id.IdeaMember.GetId().ToString()));

			var ideaMember = GetMaybe<IIdeaMember1D>(intIdentifier);
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
