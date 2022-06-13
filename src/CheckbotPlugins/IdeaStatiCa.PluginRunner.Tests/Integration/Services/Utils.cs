using Grpc.Core;

namespace IdeaStatiCa.PluginRunner.Tests.Integration.Services
{
	internal static class Utils
	{
		public static AsyncUnaryCall<T> CreateUnaryCall<T>(T result)
		{
			return new(
				Task.FromResult(result),
				null,
				null,
				null,
				  null);
		}
	}
}