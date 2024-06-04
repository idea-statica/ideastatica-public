#pragma once
#include "ImporterContext.h"

using namespace IdeaRS::OpenModel;
using namespace System::Collections::Generic;
using namespace IdeaStatiCa::BimApiLink::Plugin;
using namespace IdeaStatiCa::Plugin;

namespace CppFeaApiWrapper
{
	namespace Model
	{
		public ref class Model : IFeaModel
		{
		private:
			ImporterContext^ context;
			IProgressMessaging^ messagingService;

		public:
			Model(ImporterContext^ context, IProgressMessaging^ messagingService);
			

			// Inherited via IFeaModel
			virtual IEnumerable<IdeaStatiCa::BimApiLink::Identifiers::Identifier<IdeaStatiCa::BimApi::IIdeaMember1D^>^>^ GetAllMembers();
			virtual OriginSettings^ GetOriginSettings();
			virtual FeaUserSelection^ GetUserSelection();
			virtual void SelectUserSelection(System::Collections::Generic::IEnumerable<IdeaStatiCa::BimApiLink::Identifiers::Identifier<IdeaStatiCa::BimApi::IIdeaNode^>^>^ nodes, System::Collections::Generic::IEnumerable<IdeaStatiCa::BimApiLink::Identifiers::Identifier<IdeaStatiCa::BimApi::IIdeaMember1D^>^>^ members);
		};
	}
}

