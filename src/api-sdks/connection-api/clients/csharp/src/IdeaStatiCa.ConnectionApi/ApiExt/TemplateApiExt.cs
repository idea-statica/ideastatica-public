using IdeaStatiCa.ConnectionApi.Model;
using System.IO;
using System.Text;

namespace IdeaStatiCa.ConnectionApi.Api
{

	public interface ITemplateApiExtAsync : ITemplateApiAsync
	{
		ConTemplateMappingGetParam ImportTemplateFromFile(string fileName);
	}

	public class TemplateApiExt : TemplateApi, ITemplateApiExtAsync
	{
		public TemplateApiExt(Client.ISynchronousClient client, Client.IAsynchronousClient asyncClient, Client.IReadableConfiguration configuration) : base(client, asyncClient, configuration)
		{
		}

		public ConTemplateMappingGetParam ImportTemplateFromFile(string filePath)
		{
			ConTemplateMappingGetParam templateParams = new ConTemplateMappingGetParam();
			templateParams.Template = File.ReadAllText(filePath, Encoding.Unicode);
			return templateParams;
		}
	}
}
