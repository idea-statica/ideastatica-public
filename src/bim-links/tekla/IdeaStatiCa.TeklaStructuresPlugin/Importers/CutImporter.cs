using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.TeklaStructuresPlugin.BimApi;
using Tekla.Structures.Model;

namespace IdeaStatiCa.TeklaStructuresPlugin.Importers
{
	public class CutImporter : BaseImporter<IIdeaCut>
	{
		public CutImporter(IModelClient model, IPluginLogger plugInLogger)
			: base(model, plugInLogger)
		{
		}

		public override IIdeaCut Create(string id)
		{
			PlugInLogger.LogInformation($"CutImporter create {id}");
			var teklaCut = Model.GetItemByHandler(id);
			if (teklaCut == null)
			{
				PlugInLogger.LogInformation($"CutImporter not found member {id}");
				return null;
			}


			if (teklaCut is CutPlane cutPlane)
			{
				PlugInLogger.LogInformation("CutImporter cut by cutPlane ");
				return new Cut(cutPlane.Identifier.GUID.ToString())
				{
					ModifiedObjectNo = cutPlane.Father.Identifier.GUID.ToString(),
					CuttingObjectNo = cutPlane.Identifier.GUID.ToString(),
				};
			}
			else if (teklaCut is Fitting fitting)
			{
				PlugInLogger.LogInformation("CutImporter cut by fitting ");
				return new Cut(fitting.Identifier.GUID.ToString())
				{
					ModifiedObjectNo = fitting.Father.Identifier.GUID.ToString(),
					CuttingObjectNo = fitting.Identifier.GUID.ToString(),
				};
			}
			else if (teklaCut is BooleanPart booleanPart && booleanPart.Type == BooleanPart.BooleanTypeEnum.BOOLEAN_CUT)
			{
				PlugInLogger.LogInformation("CutImporter cut by booleanPart ");
				return new Cut(booleanPart.Identifier.GUID.ToString())
				{
					ModifiedObjectNo = booleanPart.Father.Identifier.GUID.ToString(),
					CuttingObjectNo = booleanPart.OperativePart.Identifier.GUID.ToString(),
					CutMethod = IdeaRS.OpenModel.Connection.CutMethod.Surface,
				};
			}
			else
			{
				PlugInLogger.LogInformation($"CutImporter unknown cut {teklaCut.GetType()}");
				return null;
			}
		}
	}
}