namespace IdeaStatiCa.ConnectionApi.Api
{
	public class TemplateApiExt : TemplateApi
	{
		public TemplateApiExt(IdeaStatiCa.ConnectionApi.Client.ISynchronousClient client, IdeaStatiCa.ConnectionApi.Client.IAsynchronousClient asyncClient, IdeaStatiCa.ConnectionApi.Client.IReadableConfiguration configuration) : base(client, asyncClient, configuration)
		{
		}
	}
}
