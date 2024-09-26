using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace IdeaStatiCa.Api.Common
{
	public interface IHttpClientWrapper
	{
		void AddRequestHeader(string header, string value);

		Action<string, int> ProgressLogAction { get; set; }
		Action<string> HeartBeatLogAction { get; set; }

		Task<TResult> GetAsync<TResult>(string requestUri, CancellationToken token, string acceptHeader = "application/json", bool useHeartbeatCheck = false);
		Task<TResult> PutAsync<TResult>(string requestUri, object requestData, CancellationToken token, string acceptHeader = "application/json", bool useHeartbeatCheck = false);
		Task<TResult> PostAsync<TResult>(string requestUri, object requestData, CancellationToken token, string acceptHeader = "application/json", bool useHeartbeatCheck = false);
		Task<TResult> PostAsyncStream<TResult>(string requestUri, StreamContent stream, CancellationToken token, bool useHeartbeatCheck = false);
		Task<TResult> PostAsyncForm<TResult>(string requestUri, MultipartFormDataContent form, CancellationToken token, bool useHeartbeatCheck);
		Task DeleteAsync<TResult>(string requestUri);
	}
}