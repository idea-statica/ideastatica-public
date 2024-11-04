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
		public ref class Model //: IFeaModel
		{
		private:
			ImporterContext^ context;
			IProgressMessaging^ messagingService;

		public:
			Model(ImporterContext^ context, IProgressMessaging^ messagingService);
			

			// Inherited via IFeaModel
		};
	}
}

