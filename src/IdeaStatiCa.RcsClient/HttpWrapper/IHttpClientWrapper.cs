using System;
using System.Threading.Tasks;

namespace IdeaStatiCa.RcsClient.HttpWrapper
{
	public interface IHttpClientWrapper
	{
		Action<string, int> ProgressLogAction { get; set; }
		Action<string> HeartBeatLogAction { get; set; }
		Task<TResult> GetAsync<TResult>(string requestUri, string acceptHeader = "application/json");
		Task<TResult> PostAsync<TResult>(string requestUri, object requestData, string acceptHeader = "application/json");
	}
}
