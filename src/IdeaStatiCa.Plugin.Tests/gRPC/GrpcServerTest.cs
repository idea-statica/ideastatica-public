using FluentAssertions;
using Grpc.Core;
using IdeaStatiCa.Plugin.Grpc;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin.Tests.gRPC
{
	[TestFixture]
	public class GrpcServerTest : IDisposable
	{
		private bool disposedValue;


		public GrpcServerTest()
		{
		}

		/// <summary>
		/// Test of processing messages by GrpcServer
		/// </summary>
		/// <returns></returns>
		[Test]
		public async Task GrpcServerHandleMessagesTest()
		{
			var pluginLogger = new NullLogger();
			var grpcServer = new GrpcServer(pluginLogger, new IdeaStatiCa.Plugin.Grpc.Services.GrpcService(pluginLogger));

			var streamReader = Substitute.For<IAsyncStreamReader<GrpcMessage>>();
			var streamWriter = Substitute.For<IServerStreamWriter<GrpcMessage>>();
			var context = Substitute.For<ServerCallContext>();

			List<GrpcMessage> handledMessages = new List<GrpcMessage>();
			ManualResetEvent mre = new ManualResetEvent(false);
			int readCounter = 0;
			int writeCounter = 0;

			const string messageName = "msg1";

			// prepare two messages to process
			List<GrpcMessage> inputMessages = new List<GrpcMessage>();

			var msg1 = new GrpcMessage();
			msg1.ClientId = "client1";
			msg1.MessageName = messageName;
			inputMessages.Add(msg1);

			var msg2 = new GrpcMessage();
			msg2.ClientId = "client1";
			msg2.MessageName = messageName;
			inputMessages.Add(msg2);


			var handler = Substitute.For<IGrpcMessageHandler>();
			handler.HandleServerMessage(default, default).ReturnsForAnyArgs(t =>
			{
				writeCounter++;
				handledMessages.Add(t[0] as GrpcMessage);

				if (writeCounter == inputMessages.Count)
				{
					mre.Set();
				}
				return Task.FromResult(new object());
			});

			grpcServer.GrpcService.RegisterHandler(messageName, handler);



			var readEnumerator = inputMessages.GetEnumerator();

			streamReader.MoveNext().ReturnsForAnyArgs(t =>
			{
				if (readCounter < inputMessages.Count)
				{
					readCounter++;
					return readEnumerator.MoveNext();
				}
				else
				{
					// wait till the last message is processed
					mre.WaitOne();
					return false;
				}
			});

			streamReader.Current.ReturnsForAnyArgs(t => readEnumerator.Current);

			await grpcServer.GrpcService.ConnectAsync(streamReader, streamWriter, context);

			// message handler should be called two times
			handledMessages.Count.Should().Be(2);
		}

		///// <summary>
		///// Test of GrpcServer which has more registered handles
		///// </summary>
		///// <returns></returns>
		//[Test]
		//public async Task GrpcServerTwoHandlesTest()
		//{
		//	var grpcServer = new GrpcServer(new NullLogger());

		//	var streamReader = Substitute.For<IAsyncStreamReader<GrpcMessage>>();
		//	var streamWriter = Substitute.For<IServerStreamWriter<GrpcMessage>>();
		//	var context = Substitute.For<ServerCallContext>();

		//	List<GrpcMessage> handledMsgsHandler1 = new List<GrpcMessage>();
		//	List<GrpcMessage> handledMsgsHandler2 = new List<GrpcMessage>();

		//	var handler1 = Substitute.For<IGrpcMessageHandler>();
		//	handler1.HandleServerMessage(default, default).ReturnsForAnyArgs(t =>
		//	{
		//		handledMsgsHandler1.Add(t[0] as GrpcMessage);
		//		return Task.FromResult(new object());
		//	});

		//	const string handler1msgName = "msg1";
		//	grpcServer.RegisterHandler(handler1msgName, handler1);


		//	var handler2 = Substitute.For<IGrpcMessageHandler>();
		//	handler2.HandleServerMessage(default, default).ReturnsForAnyArgs(t =>
		//	{
		//		handledMsgsHandler2.Add(t[0] as GrpcMessage);
		//		return Task.FromResult(new object());
		//	});

		//	const string handler2msgName = "msg2";
		//	grpcServer.RegisterHandler(handler2msgName, handler2);

		//	// prepare two messages to process
		//	List<GrpcMessage> inputMessages = new List<GrpcMessage>();

		//	var msg1 = new GrpcMessage();
		//	msg1.ClientId = "client1";
		//	msg1.MessageName = handler1msgName;
		//	msg1.Data = "1";
		//	inputMessages.Add(msg1);

		//	var msg2 = new GrpcMessage();
		//	msg2.ClientId = "client1";
		//	msg2.MessageName = handler2msgName;
		//	msg2.Data = "2";
		//	inputMessages.Add(msg2);

		//	var msg3 = new GrpcMessage();
		//	msg3.ClientId = "client1";
		//	msg3.MessageName = handler2msgName;
		//	msg3.Data = "3";
		//	inputMessages.Add(msg3);

		//	var readEnumerator = inputMessages.GetEnumerator();

		//	streamReader.MoveNext().ReturnsForAnyArgs(t =>
		//	{
		//		return readEnumerator.MoveNext();
		//	});

		//	streamReader.Current.ReturnsForAnyArgs(t => readEnumerator.Current);

		//	await grpcServer.ConnectAsync(streamReader, streamWriter, context);

		//	// each message handler should be called two times
		//	handledMsgsHandler1.Should().ContainSingle();
		//	handledMsgsHandler2.Count.Should().Be(2);
		//}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects)
				}

				// TODO: free unmanaged resources (unmanaged objects) and override finalizer
				// TODO: set large fields to null
				disposedValue = true;
			}
		}

		// // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
		// ~GrpcServerTest()
		// {
		//     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		//     Dispose(disposing: false);
		// }

		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
