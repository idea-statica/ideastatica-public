#pragma once

namespace BimApiLinkCppFeaExample::BimApi
{
	using namespace IdeaStatiCa::BimApi;
	using namespace IdeaStatiCa::BimApiLink::BimApi;

	/// <summary>
	/// A cross-section from the IDEA StatiCa library, whose material is imported when BimApiLink reads it.
	/// </summary>
	ref class CrossSectionByName : IdeaCrossSectionByName
	{
	public:
		CrossSectionByName(int id)
			: IdeaCrossSectionByName(id)
		{
		}

		property int MaterialId;

		property IIdeaMaterial^ Material
		{
			IIdeaMaterial^ get() override
			{
				return Get<IIdeaMaterial^>(MaterialId);
			}
		}
	};
}
