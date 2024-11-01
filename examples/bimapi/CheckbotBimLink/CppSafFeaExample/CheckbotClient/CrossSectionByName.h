#pragma once
using namespace IdeaStatiCa::BimApi;
using namespace IdeaStatiCa::BimApiLink::BimApi;

namespace CppFeaApiWrapper
{
	namespace BimApi
	{
		public ref class CrossSectionByName : IdeaCrossSectionByName
		{
		private:
			int materialNo;

		public:
			CrossSectionByName(int no);

			property IdeaStatiCa::BimApi::IIdeaMaterial^ Material
			{
				IdeaStatiCa::BimApi::IIdeaMaterial^ get() override {
					return Get<IIdeaMaterial^>(MaterialNo);
				}
			}

			property int MaterialNo {
				int get() {
					return materialNo;
				}
				void set(int value) {
					materialNo = value;
				}
			}
		};
	}
}