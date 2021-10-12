using FluentAssertions;
using IdeaStatiCa.Plugin.Grpc;
using IdeaStatiCa.Plugin.Grpc.Reflection;
using IdeaStatiCa.Plugin.ProjectContent;
using IdeaStatiCa.Public;
using Newtonsoft.Json;
using NSubstitute;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using System;

namespace IdeaStatiCa.Plugin.Tests.ProjectContent
{
	public class ProjectContentClientTest
	{
		/// <summary>
		/// Check of the calling of method <see cref="IProjectContent.GetContent"/>
		/// </summary>
		[Fact]
		public void GetContentTest()
		{
			var grpcClient = Substitute.For<IGrpcSynchronousClient>();

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

		/// <summary>
		/// Check of the calling of method <see cref="IProjectContent.Exist(string)"/>
		/// </summary>
		[Fact]
		public void ExistTest()
		{
			var grpcClient = Substitute.For<IGrpcSynchronousClient>();

			const string messageName1 = Constants.GRPC_PROJECTCONTENT_HANDLER_MESSAGE;

			var projectContentHandler = new ProjectContentClientHandler(grpcClient);

			string item1Id = "item1Id";
			string item2Id = "item2Id";
			string item3Id = "*";

			grpcClient.SendMessageDataSync(default).ReturnsForAnyArgs(t => {
				var request = (t[0] as GrpcMessage);
				

				GrpcReflectionInvokeData invokeData = JsonConvert.DeserializeObject<GrpcReflectionInvokeData>(request.Data);

				var firstParam = invokeData.Parameters.First();

				bool result = false;
				var response = new GrpcMessage(request);
				if (firstParam.Value.ToString().Equals(item1Id))
				{
					// true for 'item1Id'
					result = true;
				}
				else if(firstParam.Value.ToString().Equals(item3Id))
				{
					response.Data = "Error";
					return response;
				}
				else
				{
					// false for 'item2Id'
					result = false;
				}

				
				response.Data = JsonConvert.SerializeObject(result);

				return response;
			});

			grpcClient.RegisterHandler(messageName1, projectContentHandler);

			var isItem1 = projectContentHandler.Exist(item1Id);
			isItem1.Should().BeTrue();

			var isItem2 = projectContentHandler.Exist(item2Id);
			isItem2.Should().BeFalse();

			projectContentHandler.Invoking(o => o.Exist(item3Id)).Should().Throw<Exception>("'*' is not supported character");
		}

		/// <summary>
		/// 
		/// </summary>
		[Fact]
		public void DeleteTest()
		{
			var grpcClient = Substitute.For<IGrpcSynchronousClient>();

			const string messageName1 = Constants.GRPC_PROJECTCONTENT_HANDLER_MESSAGE;

			var projectContentHandler = new ProjectContentClientHandler(grpcClient);

			string item1Id = "item1Id";
			string item2Id = "*";

			grpcClient.SendMessageDataSync(default).ReturnsForAnyArgs(t => {
				var request = (t[0] as GrpcMessage);


				GrpcReflectionInvokeData invokeData = JsonConvert.DeserializeObject<GrpcReflectionInvokeData>(request.Data);

				var firstParam = invokeData.Parameters.First();

				bool result = false;
				var response = new GrpcMessage(request);
				if (firstParam.Value.ToString().Equals(item1Id))
				{
					response.Data = string.Empty;
				}
				else if (firstParam.Value.ToString().Equals(item2Id))
				{
					response.Data = "Error";
					return response;
				}

				response.Data = JsonConvert.SerializeObject(result);
				return response;
			});

			grpcClient.RegisterHandler(messageName1, projectContentHandler);

			projectContentHandler.Invoking(o => o.Delete(item1Id)).Should().NotThrow<Exception>("Deleting should not throw exception pass");
			projectContentHandler.Invoking(o => o.Delete(item2Id)).Should().Throw<Exception>("'*' is not supported character");
		}
	}
}
