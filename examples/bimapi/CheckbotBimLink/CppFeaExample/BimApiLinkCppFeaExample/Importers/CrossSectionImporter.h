#pragma once

#include "BimApi/CrossSectionByName.h"
#include "CppFeaExampleApi/FeaGeometryApi.h"

namespace BimApiLinkCppFeaExample::Importers
{
	using namespace CppFeaExampleApi;
	using namespace IdeaStatiCa::BimApi;
	using namespace System;

	/// <summary>
	/// Imports a cross-section by its FEA id.
	/// </summary>
	ref class CrossSectionImporter : BimApiLinkCppFeaExampleBases::CrossSectionImporterBase
	{
	public:
		CrossSectionImporter(const IFeaGeometryApi& geometry)
			: _geometry(&geometry)
		{
		}

		/// <summary>
		/// The cross-section with the given FEA id. Checkbot looks its name up in the IDEA StatiCa library.
		/// </summary>
		virtual IIdeaCrossSection^ Create(int id) override
		{
			FeaCrossSection crossSection = _geometry->GetCrossSection(id);

			BimApi::CrossSectionByName^ ideaCrossSection = gcnew BimApi::CrossSectionByName(id);
			ideaCrossSection->Name = gcnew String(crossSection.Name.c_str());
			ideaCrossSection->MaterialId = crossSection.MaterialId;
			return ideaCrossSection;
		}

	private:
		const IFeaGeometryApi* _geometry;
	};
}
