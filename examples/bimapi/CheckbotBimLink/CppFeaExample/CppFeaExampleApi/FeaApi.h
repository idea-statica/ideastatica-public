#pragma once

#include "FeaGeometryApi.h"
#include "FeaLoadsApi.h"
#include "FeaResultsApi.h"

#include <memory>

#ifdef CPPFEAEXAMPLEAPI_EXPORTS
#define CPPFEAEXAMPLEAPI_API __declspec(dllexport)
#else
#define CPPFEAEXAMPLEAPI_API __declspec(dllimport)
#endif

namespace CppFeaExampleApi
{
	/// <summary>
	/// The API of the example FEA application. A real link gets this object from the application it runs in.
	/// </summary>
	class IFeaApi
	{
	public:
		virtual ~IFeaApi() = default;

		/// <summary>
		/// Reads the structure of the model open in the application.
		/// </summary>
		virtual const IFeaGeometryApi& Geometry() const = 0;

		/// <summary>
		/// Reads the load groups, load cases and combinations of the model.
		/// </summary>
		virtual const IFeaLoadsApi& Loads() const = 0;

		/// <summary>
		/// Reads the internal forces the application has calculated.
		/// </summary>
		virtual const IFeaResultsApi& Results() const = 0;
	};

	/// <summary>
	/// Creates the example FEA application, with its fixed model and results.
	/// </summary>
	CPPFEAEXAMPLEAPI_API std::unique_ptr<IFeaApi> CreateFeaApi();
}
