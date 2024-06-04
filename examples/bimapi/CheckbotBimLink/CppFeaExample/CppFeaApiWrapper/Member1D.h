#pragma once
using namespace IdeaStatiCa::BimApi;
using namespace IdeaStatiCa::BimApiLink::BimApi;

namespace CppFeaApiWrapper
{
	namespace BimApi
	{
		public ref class Member1D : IdeaMember1D
		{
		public:
			Member1D(int no);

		public:
			property int CrossSectionNo;

			virtual property IdeaStatiCa::BimApi::IIdeaCrossSection^ CrossSection {
				IdeaStatiCa::BimApi::IIdeaCrossSection^ get() override {
					return Get<IdeaStatiCa::BimApi::IIdeaCrossSection^>(CrossSectionNo);
				}
			}
		};
	}
}