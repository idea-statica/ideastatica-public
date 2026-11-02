using IdeaStatiCa.IntermediateModel.IRModel;

namespace IdeaStatiCa.IntermediateModel
{

	// Interface for the export intermediate service
	public interface IIRExportToXMLService
	{
		/// <summary>
		/// Export Smodelinto 
		/// </summary>
		/// <param name="irModel"></param>
		/// <returns></returns>
		string ExportToXml(SModel irModel);
	}

}
