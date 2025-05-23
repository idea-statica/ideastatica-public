using IdeaStatiCa.Api.Common;
using IdeaStatiCa.RcsApi.Api;
using System;

namespace IdeaStatiCa.RcsApi
{
	/// <summary>
	/// Client for accessing IdeaStatiCa.ConnectionRestApi
	/// </summary>
	public interface IRcsApiClient : IApiClient
#if NETSTANDARD2_1_OR_GREATER
		, IAsyncDisposable
#endif
	{
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
	}
}
