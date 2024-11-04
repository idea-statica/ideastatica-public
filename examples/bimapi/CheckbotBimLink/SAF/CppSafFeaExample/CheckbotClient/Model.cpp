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

	}
}