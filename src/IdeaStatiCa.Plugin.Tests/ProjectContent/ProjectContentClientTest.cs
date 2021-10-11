using FluentAssertions;
using IdeaStatiCa.Plugin.Grpc;
using IdeaStatiCa.Plugin.ProjectContent;
using IdeaStatiCa.Public;
using Newtonsoft.Json;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace IdeaStatiCa.Plugin.Tests.ProjectContent
{
	public class ProjectContentClientTest
	{
		[Fact]
		public async Task GetContentTest()
		{
			const string clientId = "client1";
			var grpcClient = Substitute.For<IGrpcSynchronousClient>();

			GrpcMessage handler1Msg = null;

			var projContentList = new List<ProjectDataItem>();
			projContentList.Add(new ProjectDataItem("File1.xml", ItemType.File));
			projContentList.Add(new ProjectDataItem("File2.xml", ItemType.File));

			const string messageName1 = Constants.GRPC_PROJECTCONTENT_HANDLER_MESSAGE;

			var projectContentHandler = new ProjectContentClientHandler(grpcClient);

				grpcClient.SendMessageDataSync(default).ReturnsForAnyArgs(t => {
				var request = (t[0] as GrpcMessage);
				var response = new GrpcMessage(request);

				response.Data = JsonConvert.SerializeObject(projContentList);

				return response;
			});

			grpcClient.RegisterHandler(messageName1, projectContentHandler);

			var projectContentResult = projectContentHandler.GetContent();

			Assert.NotNull(projectContentResult);
			projectContentResult.Should().BeEquivalentTo(projContentList);
		}
	}
}
