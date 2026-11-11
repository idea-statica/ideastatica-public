#pragma once

#include "CppFeaExampleApi/FeaGeometryApi.h"
#include "CppFeaExampleApi/FeaLoadsApi.h"

namespace BimApiLinkCppFeaExample
{
	using namespace CppFeaExampleApi;
	using namespace IdeaRS::OpenModel;
	using namespace IdeaStatiCa::BimApi;
	using namespace IdeaStatiCa::BimApiLink::Identifiers;
	using namespace IdeaStatiCa::BimApiLink::Plugin;
	using namespace System::Collections::Generic;

	/// <summary>
	/// Tells Checkbot what the user selected in the FEA application and which combinations the model has. It answers
	/// with identifiers only; Checkbot then asks the importers for each object.
	/// </summary>
	ref class Model : IFeaModel
	{
	public:
		Model(const IFeaGeometryApi& geometry, const IFeaLoadsApi& loads)
			: _geometry(&geometry)
			, _loads(&loads)
		{
		}

		/// <summary>
		/// Information about the source model that Checkbot keeps with the import. Only the project name is set here.
		/// </summary>
		virtual OriginSettings^ GetOriginSettings()
		{
			OriginSettings^ settings = gcnew OriginSettings();
			settings->ProjectName = "C++ FEA example";
			return settings;
		}

		/// <summary>
		/// Every member of the model. BimApiLink builds a graph of nodes and members from them, to find which members
		/// meet at each node.
		/// </summary>
		virtual IEnumerable<Identifier<IIdeaMember1D^>^>^ GetAllMembers()
		{
			return ToIdentifiers(_geometry->GetAllMembers());
		}

		/// <summary>
		/// Answers Checkbot's import: the nodes and members the user selected. Each selected node becomes a connection
		/// of the selected members that meet there, and so does each node where two or more selected members meet.
		/// </summary>
		virtual FeaUserSelection^ GetUserSelection()
		{
			List<Identifier<IIdeaNode^>^>^ nodes = gcnew List<Identifier<IIdeaNode^>^>();
			for (const FeaNode& node : _geometry->GetSelectedNodes())
			{
				nodes->Add(gcnew IntIdentifier<IIdeaNode^>(node.Id));
			}

			FeaUserSelection^ selection = gcnew FeaUserSelection();
			selection->Nodes = nodes;
			selection->Members = ToIdentifiers(_geometry->GetSelectedMembers());
			return selection;
		}

		/// <summary>
		/// Every combination of the model. Checkbot imports them with the load cases and load groups they use, and
		/// the connections are checked for them.
		/// </summary>
		virtual IEnumerable<Identifier<IIdeaCombiInput^>^>^ GetAllCombinations()
		{
			List<Identifier<IIdeaCombiInput^>^>^ combinations = gcnew List<Identifier<IIdeaCombiInput^>^>();
			for (const FeaCombination& combination : _loads->GetAllCombinations())
			{
				combinations->Add(gcnew IntIdentifier<IIdeaCombiInput^>(combination.Id));
			}

			return combinations;
		}

		/// <summary>
		/// Would select the nodes and members in the FEA application. FeaBimLink never calls it, so it does nothing
		/// here.
		/// </summary>
		virtual void SelectUserSelection(IEnumerable<Identifier<IIdeaNode^>^>^, IEnumerable<Identifier<IIdeaMember1D^>^>^)
		{
		}

	private:
		static List<Identifier<IIdeaMember1D^>^>^ ToIdentifiers(const std::vector<FeaMember>& members)
		{
			List<Identifier<IIdeaMember1D^>^>^ identifiers = gcnew List<Identifier<IIdeaMember1D^>^>();
			for (const FeaMember& member : members)
			{
				identifiers->Add(gcnew IntIdentifier<IIdeaMember1D^>(member.Id));
			}

			return identifiers;
		}

		const IFeaGeometryApi* _geometry;
		const IFeaLoadsApi* _loads;
	};
}
