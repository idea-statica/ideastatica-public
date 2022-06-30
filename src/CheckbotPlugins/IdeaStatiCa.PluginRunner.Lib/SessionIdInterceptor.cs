using Grpc.Core;
using Grpc.Core.Interceptors;

namespace IdeaStatiCa.PluginRunner
{
	internal class SessionIdInterceptor : Interceptor
	{
		public const string MetadataSessionId = "X-Plugin-Session-Id";

		private readonly string _sessionId;

		public SessionIdInterceptor(string sessionId)
		{
			_sessionId = sessionId;
		}

		private CallOptions AddSessionIdToHeaders(CallOptions callOptions)
		{
			Metadata metadata;

			if (callOptions.Headers is null)
			{
				metadata = new Metadata();
				callOptions = callOptions.WithHeaders(metadata);
			}
			else
			{
				metadata = callOptions.Headers;
			}

			metadata.Add(MetadataSessionId, _sessionId);

			return callOptions;
		}

		private ClientInterceptorContext<TRequest, TResponse> GetContext<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context)
			where TRequest : class
			where TResponse : class
		{
			return new ClientInterceptorContext<TRequest, TResponse>(context.Method, context.Host, AddSessionIdToHeaders(context.Options));
		}

		public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(
			ClientInterceptorContext<TRequest, TResponse> context,
			AsyncClientStreamingCallContinuation<TRequest, TResponse> continuation)
		{
			return continuation(GetContext(context));
		}

		public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context, AsyncDuplexStreamingCallContinuation<TRequest, TResponse> continuation)
		{
			return continuation(GetContext(context));
		}

		public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
		{
			return continuation(request, GetContext(context));
		}

		public override TResponse BlockingUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
		{
			return continuation(request, GetContext(context));
		}

		public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncServerStreamingCallContinuation<TRequest, TResponse> continuation)
		{
			return continuation(request, GetContext(context));
		}
	}
}