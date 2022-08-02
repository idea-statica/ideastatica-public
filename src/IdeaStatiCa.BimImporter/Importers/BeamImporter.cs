using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Connection;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;
using System.Collections.Generic;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal class BeamImporter : AbstractImporter<IIdeaConnectedMember>
	{
		public BeamImporter(IPluginLogger logger) : base(logger)
		{
		}

		protected override object ImportInternal(IImportContext ctx, IIdeaConnectedMember member, ConnectionData connectionData)
		{
			ReferenceElement referenceMember = ctx.Import(member.IdeaMember);
			BeamData beamIOM = new BeamData()
			{
				Id = referenceMember.Id,
				RefLineInCenterOfGravity = true,
				AutoAddCutByWorkplane = true,
				IsAdded = member.ConnectedMemberType != IdeaConnectedMemberType.Structural,
				IsNegativeObject = member.ConnectedMemberType == IdeaConnectedMemberType.Negative,
				IsBearingMember = member.IsBearing,
				Name = member.IdeaMember.Name,
				OriginalModelId = member.IdeaMember.Id,
				MirrorY = member.IdeaMember.MirrorY,
			};

			if (beamIOM.IsAdded || beamIOM.IsNegativeObject)
			{
				beamIOM.AddedMember = referenceMember;
			}

			(connectionData.Beams ?? (connectionData.Beams = new List<BeamData>())).Add(beamIOM);

			return beamIOM;
		}

		protected override OpenElementId ImportInternal(IImportContext ctx, IIdeaConnectedMember obj)
		{
			throw new System.NotImplementedException();
		}
	}
}