#pragma once

#include "CppFeaExampleApi/FeaGeometryApi.h"

namespace BimApiLinkCppFeaExample::Importers
{
	using namespace CppFeaExampleApi;
	using namespace IdeaStatiCa::BimApi;
	using namespace IdeaStatiCa::BimApiLink::BimApi;
	using namespace System;

	/// <summary>
	/// Imports a material by its FEA id.
	/// </summary>
	ref class MaterialImporter : BimApiLinkCppFeaExampleBases::MaterialImporterBase
	{
	public:
		MaterialImporter(const IFeaGeometryApi& geometry)
			: _geometry(&geometry)
		{
		}

		/// <summary>
		/// The steel of the given FEA material. Checkbot looks its name up among the IDEA StatiCa library steels.
		/// </summary>
		virtual IIdeaMaterial^ Create(int id) override
		{
			FeaMaterial material = _geometry->GetMaterial(id);

			IdeaMaterialByName^ ideaMaterial = gcnew IdeaMaterialByName(id);
			ideaMaterial->Name = gcnew String(material.Name.c_str());
			ideaMaterial->MaterialType = MaterialType::Steel;
			return ideaMaterial;
		}

	private:
		const IFeaGeometryApi* _geometry;
	};
}
