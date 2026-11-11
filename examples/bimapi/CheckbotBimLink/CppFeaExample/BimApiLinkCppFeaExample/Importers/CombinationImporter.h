#pragma once

#include "CppFeaExampleApi/FeaLoadsApi.h"

namespace BimApiLinkCppFeaExample::Importers
{
	using namespace CppFeaExampleApi;
	using namespace IdeaRS::OpenModel::Loading;
	using namespace IdeaStatiCa::BimApi;
	using namespace IdeaStatiCa::BimApiLink::BimApi;
	using namespace System;
	using namespace System::Collections::Generic;

	/// <summary>
	/// Imports a combination by its FEA id, with its load cases.
	/// </summary>
	ref class CombinationImporter : BimApiLinkCppFeaExampleBases::CombinationImporterBase
	{
	public:
		CombinationImporter(const IFeaLoadsApi& loads)
			: _loads(&loads)
		{
		}

		/// <summary>
		/// Converts the combination with the given FEA id. Its terms add up: each load case multiplied by its factor.
		/// The load cases are imported by their own importer.
		/// </summary>
		virtual IIdeaCombiInput^ Create(int id) override
		{
			FeaCombination combination = _loads->GetCombination(id);

			List<IIdeaCombiItem^>^ items = gcnew List<IIdeaCombiItem^>();
			for (const FeaCombinationFactor& factor : combination.Factors)
			{
				items->Add(CreateItem(id, factor));
			}

			IdeaCombiInput^ ideaCombination = gcnew IdeaCombiInput(id);
			ideaCombination->Name = gcnew String(combination.Name.c_str());
			ideaCombination->TypeCombiEC = ToIdea(combination.LimitState);
			ideaCombination->TypeCalculationCombi = TypeCalculationCombiEC::Linear;
			ideaCombination->CombiItems = items;
			return ideaCombination;
		}

	private:
		/// <summary>
		/// One term of the combination: a load case and its factor.
		/// </summary>
		IdeaCombiItem^ CreateItem(int combinationId, const FeaCombinationFactor& factor)
		{
			IdeaCombiItem^ item = gcnew IdeaCombiItem(String::Format("{0}-{1}", combinationId, factor.LoadCaseId));
			item->LoadCase = Get<IIdeaLoadCase^>(factor.LoadCaseId);
			item->Coeff = factor.Factor;
			return item;
		}

		/// <summary>
		/// The Eurocode combination type for a limit state. The serviceability combinations here are characteristic.
		/// </summary>
		static TypeOfCombiEC ToIdea(FeaLimitState limitState)
		{
			switch (limitState)
			{
			case FeaLimitState::Ultimate:
				return TypeOfCombiEC::ULS;
			case FeaLimitState::Serviceability:
				return TypeOfCombiEC::SLS_Char;
			default:
				throw gcnew NotSupportedException(String::Format("Limit state {0} is not supported.", static_cast<int>(limitState)));
			}
		}

		const IFeaLoadsApi* _loads;
	};
}
