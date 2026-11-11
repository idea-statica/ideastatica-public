#pragma once

#include "FeaObjects.h"

#include <vector>

namespace CppFeaExampleApi
{
	/// <summary>
	/// Reads the internal forces the example FEA application has calculated. In a real link, this is the API of your
	/// application.
	/// </summary>
	class IFeaResultsApi
	{
	public:
		virtual ~IFeaResultsApi() = default;

		/// <summary>
		/// The internal forces along the member for the load case, from the start of the member to its end. Two results
		/// can be at the same position.
		/// </summary>
		virtual std::vector<FeaMemberResult> GetMemberResults(int memberId, int loadCaseId) const = 0;
	};

	/// <summary>
	/// Serves fixed internal forces for the two members and three load cases of the example model.
	/// </summary>
	class FeaResultsApi final : public IFeaResultsApi
	{
	public:
		FeaResultsApi();

		std::vector<FeaMemberResult> GetMemberResults(int memberId, int loadCaseId) const override;

	private:
		struct Row
		{
			int MemberId;
			int LoadCaseId;
			FeaMemberResult Result;
		};

		/// <summary>
		/// Adds a result. The frame lies in the XZ plane and is loaded in that plane, so only N, Vz and My are not zero.
		/// </summary>
		void Add(int memberId, int loadCaseId, double position, double n, double vz, double my);

		std::vector<Row> _rows;
	};
}
