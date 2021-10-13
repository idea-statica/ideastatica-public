using IdeaStatiCa.Plugin.Grpc.Reflection;
using IdeaStatiCa.Plugin.Utilities;
using System;
using SystemTestService;

namespace GrpcServerHost
{
	class Program
	{
		static void Main(string[] args)
		{
			int grpcPort = 0;
			if (args.Length > 0)
			{
				var firstArg = args[0];
				string portNumber = firstArg.Substring("-port:".Length);
				grpcPort = int.Parse(portNumber);
			}
			else
			{
				grpcPort = PortFinder.FindPort(50000, 50500);
			}
			
			var service = new Service();

			// Create Grpc server
			var grpcServer = new GrpcReflectionServer(service, grpcPort);
			grpcServer.Start();

			Console.WriteLine($"GrpcServer is listening on port {grpcPort}");

			var l = Console.ReadLine();
		}
	}
}
