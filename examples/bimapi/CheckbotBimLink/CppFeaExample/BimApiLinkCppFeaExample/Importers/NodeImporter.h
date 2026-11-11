#pragma once

#include "CppFeaExampleApi/FeaGeometryApi.h"

namespace BimApiLinkCppFeaExample::Importers
{
	using namespace CppFeaExampleApi;
	using namespace IdeaStatiCa::BimApi;
	using namespace IdeaStatiCa::BimApiLink::BimApi;

	/// <summary>
	/// Imports a node by its FEA id.
	/// </summary>
	ref class NodeImporter : BimApiLinkCppFeaExampleBases::NodeImporterBase
	{
	public:
		NodeImporter(const IFeaGeometryApi& geometry)
			: _geometry(&geometry)
		{
		}

		/// <summary>
		/// Creates the node at the coordinates of the FEA node.
		/// </summary>
		virtual IIdeaNode^ Create(int id) override
		{
			FeaNode node = _geometry->GetNode(id);

			IdeaNode^ ideaNode = gcnew IdeaNode(id);
			ideaNode->Vector = gcnew IdeaVector3D(node.X, node.Y, node.Z);
			return ideaNode;
		}

	private:
		const IFeaGeometryApi* _geometry;
	};
}
