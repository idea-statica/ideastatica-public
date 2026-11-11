#pragma once

#include "BimApi/Member1D.h"
#include "CppFeaExampleApi/FeaApi.h"

#include <vector>

namespace BimApiLinkCppFeaExample::Importers
{
	using namespace CppFeaExampleApi;
	using namespace IdeaRS::OpenModel::Result;
	using namespace IdeaStatiCa::BimApi;
	using namespace IdeaStatiCa::BimApiLink::Identifiers;
	using namespace IdeaStatiCa::BimApiLink::Results;
	using namespace System::Collections::Generic;

	/// <summary>
	/// Imports the internal forces of members for every load case. Checkbot builds the forces of each combination from
	/// them.
	/// </summary>
	ref class ResultsImporter : IInternalForcesImporter<IIdeaMember1D^>
	{
	public:
		ResultsImporter(const IFeaApi& fea)
			: _fea(&fea)
		{
		}

		/// <summary>
		/// The internal forces of the members for every load case, in each member's own axes. The members are the
		/// ones <see cref="MemberImporter"/> created.
		/// </summary>
		virtual IEnumerable<ResultsData<IIdeaMember1D^>^>^ GetResults(IReadOnlyList<IIdeaMember1D^>^ members)
		{
			InternalForcesBuilder<IIdeaMember1D^>^ builder =
				gcnew InternalForcesBuilder<IIdeaMember1D^>(ResultLocalSystemType::Local);

			for each (BimApi::Member1D^ member in members)
			{
				int memberId = safe_cast<int>(member->Identifier->GetId());
				double length = _fea->Geometry().GetMemberLength(memberId);

				for (const FeaLoadCase& loadCase : _fea->Loads().GetAllLoadCases())
				{
					InternalForcesBuilder<IIdeaMember1D^>::Sections^ sections =
						builder->For(member, gcnew IntIdentifier<IIdeaLoadCase^>(loadCase.Id));

					std::vector<FeaMemberResult> results = _fea->Results().GetMemberResults(memberId, loadCase.Id);
					std::vector<double> positions = GetPositions(results, length);
					for (size_t i = 0; i < results.size(); i++)
					{
						Add(sections, positions[i], results[i]);
					}
				}
			}

			return builder;
		}

	private:
		/// <summary>
		/// The FEA application gives forces in kN and moments in kNm; IDEA StatiCa takes N and Nm.
		/// </summary>
		literal double KiloToBase = 1000.0;

		/// <summary>
		/// Two positions closer than this, as a fraction of the member length, are the same position.
		/// </summary>
		literal double SamePosition = 1e-6;

		/// <summary>
		/// How far apart two results at the same position are moved, as a fraction of the member length.
		/// </summary>
		literal double Separation = 1e-5;

		/// <summary>
		/// The position of each result as a fraction of the member length, from 0 at its start to 1 at its end.
		/// </summary>
		/// <remarks>
		/// An FEA application can give two results at one position, for example on both sides of a point load. A
		/// section takes only one result per load case, so the second result is moved a tiny bit further along the
		/// member. At the end of the member, the first one is moved back instead.
		/// </remarks>
		static std::vector<double> GetPositions(const std::vector<FeaMemberResult>& results, double length)
		{
			std::vector<double> positions;
			for (const FeaMemberResult& result : results)
			{
				positions.push_back(result.Position / length);
			}

			for (size_t i = 1; i < positions.size(); i++)
			{
				if (positions[i] - positions[i - 1] < SamePosition)
				{
					if (positions[i] > 1 - SamePosition)
					{
						positions[i - 1] = positions[i] - Separation;
					}
					else
					{
						positions[i] = positions[i - 1] + Separation;
					}
				}
			}

			return positions;
		}

		/// <summary>
		/// Adds one result at the position, in N and Nm.
		/// </summary>
		static void Add(InternalForcesBuilder<IIdeaMember1D^>::Sections^ sections, double position, const FeaMemberResult& result)
		{
			sections->Add(
				position,
				result.N * KiloToBase,
				result.Vy * KiloToBase,
				result.Vz * KiloToBase,
				result.Mx * KiloToBase,
				result.My * KiloToBase,
				result.Mz * KiloToBase);
		}

		const IFeaApi* _fea;
	};
}
