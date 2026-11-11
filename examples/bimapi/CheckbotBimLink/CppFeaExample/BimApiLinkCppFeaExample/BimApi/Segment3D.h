#pragma once

namespace BimApiLinkCppFeaExample::BimApi
{
	using namespace IdeaStatiCa::BimApi;
	using namespace IdeaStatiCa::BimApiLink::BimApi;

	/// <summary>
	/// A straight member axis whose end nodes are imported when BimApiLink reads them.
	/// </summary>
	ref class Segment3D : IdeaLineSegment3D
	{
	public:
		Segment3D(int id)
			: IdeaLineSegment3D(id)
		{
		}

		property int StartNodeId;

		property int EndNodeId;

		property IIdeaNode^ StartNode
		{
			IIdeaNode^ get() override
			{
				return Get<IIdeaNode^>(StartNodeId);
			}
		}

		property IIdeaNode^ EndNode
		{
			IIdeaNode^ get() override
			{
				return Get<IIdeaNode^>(EndNodeId);
			}
		}
	};
}
