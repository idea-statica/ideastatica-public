using IdeaStatiCa.Plugin.Api.ConnectionRest.Model.Model_Template;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace ConnectionWebClient.Services
{
	public interface ITemplateMappingSetter
	{
		Task<TemplateConversions> SetAsync(TemplateConversions defaultConversions);
	}

	internal class TemplateMappingSetter : ITemplateMappingSetter
	{
		public async Task<TemplateConversions> SetAsync(TemplateConversions defaultConversions)
		{
			var defaultConversionsJson = JsonConvert.SerializeObject(defaultConversions);

			await Task.CompletedTask;

			var modifiedConversions = JsonConvert.DeserializeObject<TemplateConversions>(defaultConversionsJson);
		}
	}
}
