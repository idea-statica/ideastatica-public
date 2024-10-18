using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Connection;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;
using System.Collections.Generic;

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
				return beam;
			}


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