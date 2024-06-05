#pragma once
using namespace IdeaStatiCa::BimApi;
using namespace IdeaStatiCa::BimApiLink::BimApi;

namespace CppFeaApiWrapper
{
	namespace BimApi
	{
		public ref class Member1D : IdeaMember1D
		{
		private:
			int crossSectionNo;

		public:
			Member1D(int no);

		public:
			property int CrossSectionNo
			{
				int get() { return crossSectionNo; }
				void set(int value) { crossSectionNo = value; }
			}

			virtual property IdeaStatiCa::BimApi::IIdeaCrossSection^ CrossSection
			{
				IdeaStatiCa::BimApi::IIdeaCrossSection^ get() override {
					return Get<IdeaStatiCa::BimApi::IIdeaCrossSection^>(CrossSectionNo);
				}
			}
		};
	}
}