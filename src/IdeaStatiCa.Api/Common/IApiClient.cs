using System;
using System.Threading.Tasks;

namespace IdeaStatiCa.Api.Common
{
	public interface IApiClient : IDisposable, IAsyncDisposable
	{
		/// <summary>
		/// Innialize instance of client
		/// </summary>
		/// <returns></returns>
		Task CreateAsync();
	}
}
