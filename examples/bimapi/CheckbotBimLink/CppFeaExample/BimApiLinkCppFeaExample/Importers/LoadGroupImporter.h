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
	/// Imports a load group by its FEA id.
	/// </summary>
	ref class LoadGroupImporter : BimApiLinkCppFeaExampleBases::LoadGroupImporterBase
	{
	public:
		LoadGroupImporter(const IFeaLoadsApi& loads)
			: _loads(&loads)
		{
		}

		/// <summary>
		/// Converts the load group with the given FEA id.
		/// </summary>
		virtual IIdeaLoadGroup^ Create(int id) override
		{
			FeaLoadGroup loadGroup = _loads->GetLoadGroup(id);

			IdeaLoadGroup^ ideaLoadGroup = gcnew IdeaLoadGroup(id);
			ideaLoadGroup->Name = gcnew String(loadGroup.Name.c_str());
			ideaLoadGroup->GroupType = ToIdea(loadGroup.Type);
			return ideaLoadGroup;
		}

	private:
		/// <summary>
		/// The IDEA StatiCa load group type for an FEA load group type.
		/// </summary>
		static LoadGroupType ToIdea(FeaLoadGroupType type)
		{
			switch (type)
			{
			case FeaLoadGroupType::Permanent:
				return LoadGroupType::Permanent;
			case FeaLoadGroupType::Variable:
				return LoadGroupType::Variable;
			default:
				throw gcnew NotSupportedException(String::Format("Load group type {0} is not supported.", static_cast<int>(type)));
			}
		}

		const IFeaLoadsApi* _loads;
	};
}
