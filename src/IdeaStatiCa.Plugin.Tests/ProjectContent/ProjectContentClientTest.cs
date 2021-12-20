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
using System.IO;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin.Tests.ProjectContent
{
	public class ProjectContentClientTest
	{
		/// <summary>
		/// Test of the method ProjectContentClientHandler.GetContent
		/// </summary>
		[Fact]
		public void GetContentTest()
		{
			var grpcSender = Substitute.For<IGrpcSender>();

			var projContentList = new List<ProjectDataItem>();
			projContentList.Add(new ProjectDataItem("File1.xml", ItemType.File));
			projContentList.Add(new ProjectDataItem("File2.xml", ItemType.File));

			var projectContentHandler = new ProjectContentClientHandler(grpcSender);

			GrpcMessage response = null;
			grpcSender.SendMessageAsync(default).ReturnsForAnyArgs(t =>
			{
				var request = (t[0] as GrpcMessage);
				response = new GrpcMessage(request);
				response.Data = JsonConvert.SerializeObject(projContentList);

				projectContentHandler.HandleClientMessage(response, grpcSender);

				return Task.CompletedTask;
			});

			var projectContentResult = projectContentHandler.GetContent();

			Assert.NotNull(projectContentResult);
			projectContentResult.Should().BeEquivalentTo(projContentList);
		}

		/// <summary>
		/// Test of the method ProjectContentClientHandler.Exist
		/// </summary>
		[Fact]
		public void ExistTest()
		{
			var grpcSender = Substitute.For<IGrpcSender>();

			var projectContentHandler = new ProjectContentClientHandler(grpcSender);

			string item1Id = "item1Id";
			string item2Id = "item2Id";
			string item3Id = "*";

			grpcSender.SendMessageAsync(default).ReturnsForAnyArgs(t =>
			{
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
				else if (firstParam.Value.ToString().Equals(item3Id))
				{
					response.Data = "Error";

					projectContentHandler.HandleClientMessage(response, grpcSender);
					return Task.CompletedTask;
				}
				else
				{
					// false for 'item2Id'
					result = false;
				}

				response.Data = JsonConvert.SerializeObject(result);
				projectContentHandler.HandleClientMessage(response, grpcSender);

				//return response;
				return Task.CompletedTask;
			});

			var isItem1 = projectContentHandler.Exist(item1Id);
			isItem1.Should().BeTrue();

			var isItem2 = projectContentHandler.Exist(item2Id);
			isItem2.Should().BeFalse();

			projectContentHandler.Invoking(o => o.Exist(item3Id)).Should().Throw<Exception>("'*' is not supported character");
		}

		/// <summary>
		/// Test of the method ProjectContentClientHandler.Delete
		/// </summary>
		[Fact]
		public void DeleteTest()
		{
			var grpcSender = Substitute.For<IGrpcSender>();

			var projectContentHandler = new ProjectContentClientHandler(grpcSender);

			string item1Id = "item1Id";
			string item2Id = "*";

			grpcSender.SendMessageAsync(default).ReturnsForAnyArgs(t =>
			{
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
					projectContentHandler.HandleClientMessage(response, grpcSender);
					return Task.CompletedTask;
				}

				response.Data = JsonConvert.SerializeObject(result);
				projectContentHandler.HandleClientMessage(response, grpcSender);
				return Task.CompletedTask;
			});



			projectContentHandler.Invoking(o => o.Delete(item1Id)).Should().NotThrow<Exception>("Deleting should not throw exception pass");
			projectContentHandler.Invoking(o => o.Delete(item2Id)).Should().Throw<Exception>("'*' is not supported character");
		}

		///// <summary>
		///// Test of the method ProjectContentClientHandler.ReadData
		///// </summary>
		//[Fact]
		//public void ReadDataTest()
		//{
		//	var grpcClient = Substitute.For<IGrpcSender>();

		//	const string messageName1 = Constants.GRPC_PROJECTCONTENT_HANDLER_MESSAGE;

		//	var projectContentHandler = new ProjectContentClientHandler(grpcClient);

		//	string item1Id = "item1Id";
		//	string notExistingItemId = "notExistingItemId";

		//	// create the array of 5 bytes and write original values which will be checked 
		//	int bufferSize = 5;
		//	byte[] buffer = new byte[bufferSize];
		//	for (int i = 0; i < bufferSize; i++)
		//	{
		//		buffer[i] = Convert.ToByte(i);
		//	}

		//	grpcClient.SendMessageDataSync(default).ReturnsForAnyArgs(t =>
		//	{
		//		var request = (t[0] as GrpcMessage);

		//		GrpcReflectionInvokeData invokeData = JsonConvert.DeserializeObject<GrpcReflectionInvokeData>(request.Data);

		//		var firstParam = invokeData.Parameters.First();
		//		var response = new GrpcMessage(request);

		//		if (firstParam.Value.ToString() == item1Id)
		//		{
		//			if (firstParam.Value.ToString().Equals(item1Id))
		//			{
		//				response.Data = string.Empty;
		//				response.Buffer = Google.Protobuf.ByteString.CopyFrom(buffer);
		//			}
		//		}
		//		else
		//		{
		//			response.Data = "Error";
		//		}

		//		return response;
		//	});

		//	grpcClient.RegisterHandler(messageName1, projectContentHandler);

		//	using (var resultStream = new MemoryStream())
		//	{
		//		projectContentHandler.ReadData(item1Id, resultStream);
		//		resultStream.Seek(0, SeekOrigin.Begin);

		//		resultStream.Length.Should().Be(bufferSize);
		//		for (int i = 0; i < resultStream.Length; i++)
		//		{
		//			byte b = (byte)resultStream.ReadByte();
		//			b.Should().Be(buffer[i]);
		//		}
		//	}

		//	using (var resultStream = new MemoryStream())
		//	{
		//		projectContentHandler.Invoking(o => o.ReadData(notExistingItemId, resultStream)).Should().Throw<Exception>("Exception should be thrown for not existing content");
		//	}
		//}

		///// <summary>
		///// Test of the method ProjectContentClientHandler.WriteData
		///// </summary>
		//[Fact]
		//public void WriteDataTest()
		//{
		//	var grpcClient = Substitute.For<IGrpcSender>();

		//	const string messageName1 = Constants.GRPC_PROJECTCONTENT_HANDLER_MESSAGE;

		//	var projectContentHandler = new ProjectContentClientHandler(grpcClient);

		//	string item1Id = "item1Id";

		//	// create the array of 5 bytes and write original values which will be checked 
		//	int bufferSize = 5;
		//	using (var inputDataStream = new MemoryStream())
		//	{
		//		for (int i = 0; i < bufferSize; i++)
		//		{
		//			inputDataStream.WriteByte(Convert.ToByte(i));
		//		}
		//		inputDataStream.Seek(0, SeekOrigin.Begin);

		//		using (var sentDataStream = new MemoryStream())
		//		{

		//			grpcClient.SendMessageDataSync(default).ReturnsForAnyArgs(t =>
		//			{
		//				var request = (t[0] as GrpcMessage);

		//				GrpcReflectionInvokeData invokeData = JsonConvert.DeserializeObject<GrpcReflectionInvokeData>(request.Data);

		//				var firstParam = invokeData.Parameters.First();
		//				var response = new GrpcMessage(request);

		//				if (firstParam.Value.ToString() == item1Id)
		//				{
		//					if (firstParam.Value.ToString().Equals(item1Id))
		//					{
		//						request.Buffer.WriteTo(sentDataStream);
		//						response.Data = "OK";
		//					}
		//				}
		//				else
		//				{
		//					response.Data = "Error";
		//				}

		//				return response;
		//			});

		//			grpcClient.RegisterHandler(messageName1, projectContentHandler);


		//			projectContentHandler.WriteData(item1Id, inputDataStream);
		//			sentDataStream.Seek(0, SeekOrigin.Begin);

		//			sentDataStream.Length.Should().Be(bufferSize);
		//			for (byte i = 0; i < bufferSize; i++)
		//			{
		//				var b = (byte)sentDataStream.ReadByte();
		//				b.Should().Be(i);
		//			}
		//		}
		//	}
		//}

		//[Fact]
		//public void GetStreamTest()
		//{
		//	var grpcClient = Substitute.For<IGrpcSynchronousClient>();

		//	const string messageName1 = Constants.GRPC_PROJECTCONTENT_HANDLER_MESSAGE;

		//	var projectContentHandler = new ProjectContentClientHandler(grpcClient);

		//	string item1Id = "item1Id";

		//	// create the array of 5 bytes and write original values which will be checked 
		//	int bufferSize = 5;
		//	byte[] buffer = new byte[bufferSize];
		//	for (int i = 0; i < bufferSize; i++)
		//	{
		//		buffer[i] = Convert.ToByte(i);
		//	}


		//	GrpcMessage writtenMessage = null;

		//	grpcClient.SendMessageDataSync(default).ReturnsForAnyArgs(t =>
		//	{
		//		var request = (t[0] as GrpcMessage);

		//		GrpcReflectionInvokeData invokeData = JsonConvert.DeserializeObject<GrpcReflectionInvokeData>(request.Data);

		//		var firstParam = invokeData.Parameters.First();
		//		var response = new GrpcMessage(request);
		//		if (invokeData.MethodName == "Read")
		//		{
		//			if (firstParam.Value.ToString() == item1Id)
		//			{
		//				if (firstParam.Value.ToString().Equals(item1Id))
		//				{
		//					response.Data = string.Empty;
		//					response.Buffer = Google.Protobuf.ByteString.CopyFrom(buffer);
		//				}
		//			}
		//			else
		//			{
		//				response.Data = "Error";
		//			}
		//		}
		//		else if (invokeData.MethodName == "Write")
		//		{
		//			if (firstParam.Value.ToString() == item1Id)
		//			{
		//				if (firstParam.Value.ToString().Equals(item1Id))
		//				{
		//					// store message for validation
		//					writtenMessage = request;
		//					response.Data = string.Empty;
		//					response.Buffer = Google.Protobuf.ByteString.Empty;
		//				}
		//			}
		//			else
		//			{
		//				response.Data = "Error";
		//			}
		//		}

		//		return response;
		//	});

		//	grpcClient.RegisterHandler(messageName1, projectContentHandler);

		//	using (var content1Stream = projectContentHandler.Get(item1Id))
		//	{
		//		content1Stream.Length.Should().Be(bufferSize, "Expecting data from buffer in the stream");

		//		writtenMessage.Should().BeNull();

		//		content1Stream.Seek(0, SeekOrigin.End);
		//		content1Stream.WriteByte(6);

		//		writtenMessage.Should().BeNull();
		//	}

		//	writtenMessage.Should().NotBeNull();
		//	var writtenBytes = writtenMessage.Buffer.ToByteArray();
		//	writtenBytes.Length.Should().Be(6);
		//}
	}
}
