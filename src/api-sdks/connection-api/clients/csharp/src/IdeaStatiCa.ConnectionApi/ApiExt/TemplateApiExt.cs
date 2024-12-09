using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi.Model;
using System.IO;
using System.Text;

namespace IdeaStatiCa.ConnectionApi.Api
{
	/// <summary>
	/// Connection REST API Template API extension methods. 
	/// </summary>
	public interface ITemplateApiExtAsync : ITemplateApiAsync
	{
		/// <summary>
		/// Import a IDEA StatiCa Connection template file (.contemp) from disc.
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		ConTemplateMappingGetParam ImportTemplateFromFile(string fileName);
	}

	/// <inheritdoc cref="ITemplateApiExtAsync"/>
	public class TemplateApiExt : TemplateApi, ITemplateApiExtAsync
	{
		internal TemplateApiExt(Client.ISynchronousClient client, Client.IAsynchronousClient asyncClient, Client.IReadableConfiguration configuration) : base(client, asyncClient, configuration)
		{
		}

		/// <inheritdoc cref="ITemplateApiExtAsync.ImportTemplateFromFile(string)"/>
		public ConTemplateMappingGetParam ImportTemplateFromFile(string filePath)
		{
			ConTemplateMappingGetParam templateParams = new ConTemplateMappingGetParam();
			templateParams.Template = File.ReadAllText(filePath, Encoding.Unicode);
			return templateParams;
		}
	}
}
