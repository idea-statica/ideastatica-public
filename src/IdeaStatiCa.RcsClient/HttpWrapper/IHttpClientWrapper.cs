using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace IdeaStatiCa.RcsClient.HttpWrapper
{
	public interface IHttpClientWrapper
	{
		Action<string, int> ProgressLogAction { get; set; }
		Action<string> HeartBeatLogAction { get; set; }

		Task<TResult> GetAsync<TResult>(string requestUri, CancellationToken token, string acceptHeader = "application/json");
		Task<TResult> PutAsync<TResult>(string requestUri, object requestData, CancellationToken token, string acceptHeader = "application/json");
		Task<TResult> PostAsync<TResult>(string requestUri, object requestData, CancellationToken token, string acceptHeader = "application/json");
		Task<TResult> PostAsyncStream<TResult>(string requestUri, StreamContent stream, CancellationToken token);
	}
}
