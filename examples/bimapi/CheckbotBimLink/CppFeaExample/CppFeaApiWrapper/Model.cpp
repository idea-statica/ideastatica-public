#include "pch.h"
#include "Model.h"

using namespace System::Collections::Generic;
using namespace IdeaStatiCa::BimApiLink::Identifiers;
using namespace IdeaStatiCa::BimApi;

namespace CppFeaApiWrapper
{
	namespace Model
	{

		Model::Model(ImporterContext^ context, IProgressMessaging^ messagingService)
		{
			this->context = context;
			this->messagingService = messagingService;
		}

		IEnumerable<Identifier<IIdeaMember1D^>^>^ Model::GetAllMembers()
		{
			std::vector<int> member_vec = context->GetGeometry()->GetMembersIdentifiers();
			List<Identifier<IIdeaMember1D^>^>^ memberIdentifiers = gcnew List<Identifier<IIdeaMember1D^>^>();

			for (const auto& num : member_vec)
			{
				memberIdentifiers->Add(gcnew IntIdentifier<IIdeaMember1D^>(num));
			}

			return memberIdentifiers;
		}

		OriginSettings^ Model::GetOriginSettings()
		{
			OriginSettings^ orig = gcnew OriginSettings();
			orig->ProjectName = "TestProject";

			return orig;
		}

		/// <summary>
		/// Read identfiers of nodes and members from the FEA context
		/// Convert them to identifiers and return them as <see cref="FeaUserSelection"/>
		/// </summary>
		/// <returns>Current selection from FEA in <see cref="FeaUserSelection"/> </returns>
		FeaUserSelection^ Model::GetUserSelection()
		{
			std::vector<int> node_vec = context->GetGeometry()->GetNodesIdentifiers();
			List<Identifier<IIdeaNode^>^>^ nodeIdentifiers = gcnew List<Identifier<IIdeaNode^>^>();

			for (const auto& num : node_vec)
			{
				nodeIdentifiers->Add(gcnew IntIdentifier<IdeaStatiCa::BimApi::IIdeaNode^>(num));
				List<Identifier<IIdeaNode^>^>^ nodeIdentifiers = gcnew List<Identifier<IIdeaNode^>^>();
			}

			std::vector<int> member_vec = context->GetGeometry()->GetMembersIdentifiers();
			List<Identifier<IIdeaMember1D^>^>^ memberIdentifiers = gcnew List<Identifier<IIdeaMember1D^>^>();

			for (const auto& num : member_vec)
			{
				memberIdentifiers->Add(gcnew IntIdentifier<IdeaStatiCa::BimApi::IIdeaMember1D^>(num));
			}

			FeaUserSelection^ res = gcnew FeaUserSelection();
			res->Members = memberIdentifiers;
			res->Nodes = nodeIdentifiers;

			return res;
		}

		void Model::SelectUserSelection(IEnumerable<Identifier<IdeaStatiCa::BimApi::IIdeaNode^>^>^ nodes, IEnumerable<Identifier<IdeaStatiCa::BimApi::IIdeaMember1D^>^>^ members)
		{
			throw gcnew System::NotImplementedException();
		}
	}
}