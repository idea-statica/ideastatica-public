#pragma once

#include "CppFeaExampleApi/FeaLoadsApi.h"

namespace BimApiLinkCppFeaExample::Importers
{
	using namespace CppFeaExampleApi;
	using namespace IdeaRS::OpenModel::Loading;
	using namespace IdeaStatiCa::BimApi;
	using namespace IdeaStatiCa::BimApiLink::BimApi;
	using namespace System;

	/// <summary>
	/// Imports a load case by its FEA id, with its load group.
	/// </summary>
	ref class LoadCaseImporter : BimApiLinkCppFeaExampleBases::LoadCaseImporterBase
	{
	public:
		LoadCaseImporter(const IFeaLoadsApi& loads)
			: _loads(&loads)
		{
		}

		/// <summary>
		/// Converts the load case with the given FEA id. Its load group is imported by its own importer.
		/// </summary>
		virtual IIdeaLoadCase^ Create(int id) override
		{
			FeaLoadCase loadCase = _loads->GetLoadCase(id);

			IdeaLoadCase^ ideaLoadCase = gcnew IdeaLoadCase(id);
			ideaLoadCase->Name = gcnew String(loadCase.Name.c_str());
			ideaLoadCase->LoadType = ToIdeaType(loadCase.Type);
			ideaLoadCase->Type = ToIdeaSubType(loadCase.Type);
			ideaLoadCase->Variable = VariableType::Standard;
			ideaLoadCase->LoadGroup = Get<IIdeaLoadGroup^>(loadCase.LoadGroupId);
			return ideaLoadCase;
		}

	private:
		/// <summary>
		/// Whether IDEA StatiCa treats the load case as permanent or variable.
		/// </summary>
		static LoadCaseType ToIdeaType(FeaLoadCaseType type)
		{
			switch (type)
			{
			case FeaLoadCaseType::SelfWeight:
			case FeaLoadCaseType::DeadLoad:
				return LoadCaseType::Permanent;
			case FeaLoadCaseType::Snow:
				return LoadCaseType::Variable;
			default:
				throw gcnew NotSupportedException(String::Format("Load case type {0} is not supported.", static_cast<int>(type)));
			}
		}

		/// <summary>
		/// The more detailed kind of the load case in IDEA StatiCa.
		/// </summary>
		static LoadCaseSubType ToIdeaSubType(FeaLoadCaseType type)
		{
			switch (type)
			{
			case FeaLoadCaseType::SelfWeight:
				return LoadCaseSubType::PermanentSelfweight;
			case FeaLoadCaseType::DeadLoad:
				return LoadCaseSubType::PermanentStandard;
			case FeaLoadCaseType::Snow:
				return LoadCaseSubType::VariableStatic;
			default:
				throw gcnew NotSupportedException(String::Format("Load case type {0} is not supported.", static_cast<int>(type)));
			}
		}

		const IFeaLoadsApi* _loads;
	};
}
