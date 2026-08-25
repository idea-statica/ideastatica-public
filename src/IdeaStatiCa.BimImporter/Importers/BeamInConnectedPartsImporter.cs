using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Connection;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal class BeamInConnectedPartsImporter : AbstractImporter<IIdeaMember1D>
	{
		public BeamInConnectedPartsImporter(IPluginLogger logger) : base(logger)
		{
		}

		protected override object ImportInternal(IImportContext ctx, IIdeaMember1D member, ConnectionData connectionData)
		{
			//check if reference item was imported
			var beam = connectionData.Beams.Find(b => b.OriginalModelId == member.Id);
			if (beam != null)
			{
				Logger?.LogTrace($"BeamInConnectedPartsImporter: member={member.Id} found existing beam IsAdded={beam.IsAdded} — returning");
				return beam;
			}

			// If this member is already a structural member of this connection (IsAdded=false),
			// return it directly — do not add a duplicate IsAdded=true entry.
			var structuralBeam = connectionData.Beams.Find(b => b.OriginalModelId == member.Id && !b.IsAdded);
			if (structuralBeam != null)
			{
				Logger?.LogTrace($"BeamInConnectedPartsImporter: member={member.Id} found structural beam — returning without IsAdded=true");
				return structuralBeam;
			}

			Logger?.LogTrace($"BeamInConnectedPartsImporter: member={member.Id} not found in Beams (count={connectionData.Beams?.Count}) — will create IsAdded=true. Existing IDs: {string.Join(", ", connectionData.Beams?.Select(b => b.OriginalModelId) ?? System.Linq.Enumerable.Empty<string>())}");


			ReferenceElement referenceMember = ctx.Import(member);
			BeamData beamIOM = new BeamData()
			{
				Id = referenceMember.Id,
				AutoAddCutByWorkplane = true,
				IsAdded = true,
				IsNegativeObject = false,
				IsBearingMember = false,
				Name = member.Name,
				OriginalModelId = member.Id,
				MirrorY = member.MirrorY,
				Cuts = new List<CutData>(),
				Plates = new List<PlateData>(),
			};

			if (beamIOM.IsAdded || beamIOM.IsNegativeObject)
			{
				beamIOM.AddedMember = referenceMember;
			}

			if (connectionData.Beams == null || !connectionData.Beams.Exists(b => b.OriginalModelId == beamIOM.OriginalModelId))
			{
				(connectionData.Beams ?? (connectionData.Beams = new List<BeamData>())).Add(beamIOM);
			}

			return beamIOM;
		}

		protected override OpenElementId ImportInternal(IImportContext ctx, IIdeaMember1D obj)
		{
			throw new System.NotImplementedException();
		}
	}
}