#pragma once
using namespace IdeaStatiCa::BimApi;
using namespace IdeaStatiCa::BimApiLink::BimApi;

namespace CppFeaApiWrapper
{
	namespace BimApi
	{
		public ref class Segment3D : IdeaLineSegment3D
		{
		private:
			int startNodeNo;
			int endNodeNo;

		public:
			Segment3D(int id);

			property IIdeaNode^ StartNode
			{
				IIdeaNode^ get() override { return Get<IIdeaNode^>(StartNodeNo); }
			}

			property IIdeaNode^ EndNode
			{
				IIdeaNode^ get() override { return Get<IIdeaNode^>(EndNodeNo); }
			}

			property int StartNodeNo
			{
				int get() { return startNodeNo; }
				void set(int value) { startNodeNo = value; }
			}

			property int EndNodeNo
			{
				int get() { return endNodeNo; }
				void set(int value) { endNodeNo = value; }
			}
		};
	}
}
