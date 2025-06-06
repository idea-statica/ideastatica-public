﻿using IdeaStatiCa.Api.Common;
using IdeaStatiCa.ConnectionApi.Api;
using System;

namespace IdeaStatiCa.ConnectionApi
{
	/// <summary>
	/// Client for accessing IdeaStatiCa.ConnectionRestApi
	/// </summary>
	public interface IConnectionApiClient : IApiClient
#if NETSTANDARD2_1_OR_GREATER
		, IAsyncDisposable
#endif
	{
		/// <summary>
		/// Id of the actively open project on the service side
		/// </summary>
		Guid ActiveProjectId { get; }

		/// <summary>
		/// Get unique identifier for the client
		/// </summary>
		string ClientId { get; }

		/// <summary>
		/// Get Client API. 
		/// </summary>
		IClientApi ClientApi { get; }

		/// <summary>
		/// Get Calculation API
		/// </summary>
		ICalculationApiAsync Calculation { get; }

		/// <summary>
		/// Get Connection API
		/// </summary>
		IConnectionApiAsync Connection { get; }

		/// <summary>
		/// Get Export API
		/// </summary>
		IExportApiExtAsync Export { get; }

		/// <summary>
		/// Get LoadEffect API
		/// </summary>
		ILoadEffectApiAsync LoadEffect { get; }

		/// <summary>
		/// Get Material API
		/// </summary>
		IMaterialApiAsync Material { get; }

		/// <summary>
		/// Get Member API
		/// </summary>
		IMemberApiAsync Member { get; }

		/// <summary>
		/// Get Operation API
		/// </summary>
		IOperationApiAsync Operation { get; }

		/// <summary>
		/// Get Parameter API
		/// </summary>
		IParameterApiAsync Parameter { get; }

		/// <summary>
		/// Get Presentation API
		/// </summary>
		IPresentationApiAsync Presentation { get; }

		/// <summary>
		/// Get Project API
		/// </summary>
		IProjectApiExtAsync Project { get; }

		/// <summary>
		/// Get Report API
		/// </summary>
		IReportApiExtAsync Report { get; }

		/// <summary>
		/// Get Template API
		/// </summary>
		ITemplateApiExtAsync Template { get; }

		/// <summary>
		/// Get Conversion API
		/// </summary>
		IConversionApiAsync Conversion { get; }
	}
}
