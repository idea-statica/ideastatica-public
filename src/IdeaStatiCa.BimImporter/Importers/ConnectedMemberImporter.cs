using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Connection;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal class ConnectedMemberImporter : AbstractImporter<IIdeaConnectedMember>
	{
		public ConnectedMemberImporter(IPluginLogger logger) : base(logger)
		{
		}

		protected override OpenElementId ImportInternal(IImportContext ctx, IIdeaConnectedMember connectedMember)
		{
			var x = ctx.Import(connectedMember.IdeaMember);
			var cm = new ConnectedMember()
			{
				Id = x.Id,
				MemberId = x,
				IsContinuous = connectedMember.GeometricalType == IdeaGeometricalType.Continuous,
			};
			return cm;
		}
	}
}