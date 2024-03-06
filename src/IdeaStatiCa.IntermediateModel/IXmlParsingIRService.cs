using IdeaStatiCa.IntermediateModel.IRModel;
using System.IO;

namespace IdeaStatiCa.IntermediateModel
{
	// Interface for the intermediate service
	public interface IXmlParsingIRService
	{
		/// <summary>
		/// Parse Xml
		/// </summary>
		/// <param name="xmlContent"></param>
		/// <returns></returns>
		SModel ParseXml(string xmlContent);

		/// <summary>
		/// Parse Xml
		/// </summary>
		/// <param name="streamContent"></param>
		/// <returns></returns>
		SModel ParseXml(Stream streamContent);
	}
}
