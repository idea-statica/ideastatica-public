using IdeaStatiCa.IntermediateModel.IRModel;

namespace IdeaStatiCa.IntermediateModel
{

	// Interface for the export intermediate service
	public interface IIRExportToXMLService
	{
		string ExportToXml(SModel irModel);
	}

}
