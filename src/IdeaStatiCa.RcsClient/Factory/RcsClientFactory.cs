using IdeaStatiCa.Api.Common;
using IdeaStatiCa.Api.RCS;
using IdeaStatiCa.Plugin;

namespace IdeaStatiCa.RcsClient.Factory
{
	public class RcsClientFactory : RcsClientFactoryObsolete, IRcsClientFactory
	{
		public RcsClientFactory(string isSetupDir, IPluginLogger pluginLogger = null, IHttpClientWrapper httpClientWrapper = null)
			: base(isSetupDir, pluginLogger, httpClientWrapper)
		{
			//Postfix with version for Rcs.Rest.Api
			API_VERSION_V1 = "/api/1.0/";
		}
	}
}
