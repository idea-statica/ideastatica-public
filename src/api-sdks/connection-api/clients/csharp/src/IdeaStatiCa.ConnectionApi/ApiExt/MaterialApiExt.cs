using System;
using System.Collections.Generic;
using System.Text;

namespace IdeaStatiCa.ConnectionApi.Api
{
	public class MaterialApiExt : MaterialApi
	{
			public MaterialApiExt(IdeaStatiCa.ConnectionApi.Client.ISynchronousClient client, IdeaStatiCa.ConnectionApi.Client.IAsynchronousClient asyncClient, IdeaStatiCa.ConnectionApi.Client.IReadableConfiguration configuration) : base(client, asyncClient, configuration)
			{
			}
	}
}
