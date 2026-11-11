#include "FeaApi.h"

namespace CppFeaExampleApi
{
	namespace
	{
		/// <summary>
		/// The example FEA application, with its fixed model and results.
		/// </summary>
		class FeaApi final : public IFeaApi
		{
		public:
			const IFeaGeometryApi& Geometry() const override
			{
				return _geometry;
			}

			const IFeaLoadsApi& Loads() const override
			{
				return _loads;
			}

			const IFeaResultsApi& Results() const override
			{
				return _results;
			}

		private:
			FeaGeometryApi _geometry;
			FeaLoadsApi _loads;
			FeaResultsApi _results;
		};
	}

	std::unique_ptr<IFeaApi> CreateFeaApi()
	{
		return std::make_unique<FeaApi>();
	}
}
