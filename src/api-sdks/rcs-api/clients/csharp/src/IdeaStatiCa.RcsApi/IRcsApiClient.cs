using IdeaStatiCa.RcsApi.Api;
using System;
using System.Threading.Tasks;

namespace IdeaStatiCa.RcsApi
{
	/// <summary>
	/// Client for accessing IdeaStatiCa.ConnectionRestApi
	/// </summary>
	public interface IRcsApiClient : IDisposable
#if NETSTANDARD2_1_OR_GREATER
		, IAsyncDisposable
#endif
	{
		///// <summary>
		///// ClientID - assigned by the service
		///// </summary>
		//string ClientId { get; }

		/// <summary>
		/// Get Calculation API
		/// </summary>
		ICalculationApiAsync Calculation { get; }

		/// <summary>
		/// Get CrossSection API
		/// </summary>
		ICrossSectionApiAsync CrossSection { get; }

		/// <summary>
		/// Get DesignMember API
		/// </summary>
		IDesignMemberApiAsync DesignMember { get; }

		/// <summary>
		/// Get InternalForces API
		/// </summary>
		IInternalForcesApiAsync InternalForces { get; }

		/// <summary>
		/// Get Project API
		/// </summary>
		IProjectApiExtAsync Project { get; }

		/// <summary>
		/// Get Section API
		/// </summary>
		ISectionApiAsync Section { get; }

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		Task CreateAsync();
	}
}
