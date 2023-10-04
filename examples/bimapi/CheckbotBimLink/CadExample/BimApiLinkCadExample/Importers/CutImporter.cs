using BimApiLinkCadExample.BimApi;
using BimApiLinkCadExample.CadExampleApi;
using BimApiLinkCadExample.Importers;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;

namespace BimApiLinkCadExample.BimApi
{
	internal class CutImporter : BaseImporter<IIdeaCut>
	{
		public CutImporter(ICadGeometryApi model)
			: base(model)
		{
		}

		public override IIdeaCut Create(int id)
		{
			var item = Model.GetCutByPart(id);

			Cut ideaCut = new Cut(id);

			ideaCut.CuttingObjectNo = item.CuttingPart.PartId;
			ideaCut.ModifiedObjectNo = item.PartToCut.PartId;

			ideaCut.CutMethod = IdeaRS.OpenModel.Connection.CutMethod.BoundingBox;
			ideaCut.CutOrientation = IdeaRS.OpenModel.Connection.CutOrientation.Parallel;
			ideaCut.DistanceComparison = IdeaRS.OpenModel.Connection.DistanceComparison.Closer;

			return ideaCut;
		}
	}
}