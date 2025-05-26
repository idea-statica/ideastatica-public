using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IdeaStatiCa.Api.Common
{
	public interface IApiClient : IDisposable
	{
		/// <summary>
		/// Innialize instance of client
		/// </summary>
		/// <returns></returns>
		Task CreateAsync();
	}
}
