using IdeaStatiCa.IntermediateModel.IRModel;

namespace IdeaStatiCa.IntermediateModel
{
	// Interface for the intermediate service
	public interface IXmlParsingIRService
	{
		SModel ParseXml(string xmlContent);
	}
}
