using IdeaStatiCa.Plugin;
using IdeaStatiCa.Plugin.Grpc.Reflection;
using IdeaStatiCa.Plugin.ProjectContent;
using IdeaStatiCa.Plugin.Utilities;
using System;
using System.Threading;
using SystemTestService;

namespace GrpcServerHost
{
	class Program
	{
		static void Main(string[] args)
		{
			//System.Diagnostics.Debug.Assert(false);
			int grpcPort = 0;
			string eventName = string.Empty;
			if (args.Length > 0)
			{
				// the first argument is grpcServerPort, the second is event name
				if(args.Length != 2)
				{
					throw new Exception("Expecting 2 argument - the first argument is grpcServerPort, the second is event name");
				}

				// parse the first argument is tcp port
				var firstArg = args[0];
				Console.WriteLine($"First argument : '${firstArg}'");
				string portNumber = firstArg.Substring("-grpcPort:".Length);
				grpcPort = int.Parse(portNumber);

				// parse the second argument is event name
				var secondArg = args[1];
				Console.WriteLine($"Second argument : '${secondArg}'");
				eventName = secondArg.Substring("-startEvent:".Length);

				Console.WriteLine($"Parsed arguments grpcPort = {grpcPort}, eventName = '{eventName}'");
			}
			else
			{
				grpcPort = PortFinder.FindPort(50000, 50500);
			}
			
			var service = new Service();

			// Create Grpc server
			var grpcServer = new GrpcReflectionServer(service, new NullLogger());

			var projectContentInMemory = new ProjectContentInMemory();
			var contentHandler = new ProjectContentServerHandler(projectContentInMemory);
			grpcServer.RegisterHandler(IdeaStatiCa.Plugin.Constants.GRPC_PROJECTCONTENT_HANDLER_MESSAGE, contentHandler);

			grpcServer.Connect(null, grpcPort);

			Console.WriteLine($"GrpcServer is listening on port '{grpcPort}'");

			EventWaitHandle syncEvent;
			if (EventWaitHandle.TryOpenExisting(eventName, out syncEvent))
			{
				Console.WriteLine($"Setting event '{eventName}'");
				syncEvent.Set();
				syncEvent.Dispose();
			}
			else
			{
				Console.WriteLine($"Event doesn't exist '{eventName}'");
			}

			Console.WriteLine("Enter any line to finish");

			var l = Console.ReadLine();
		}
	}
}
