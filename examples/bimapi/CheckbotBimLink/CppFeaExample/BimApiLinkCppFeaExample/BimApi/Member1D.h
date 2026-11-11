#pragma once

namespace BimApiLinkCppFeaExample::BimApi
{
	using namespace IdeaStatiCa::BimApi;
	using namespace IdeaStatiCa::BimApiLink::BimApi;

	/// <summary>
	/// A member whose cross-section is imported when BimApiLink reads it.
	/// </summary>
	ref class Member1D : IdeaMember1D
	{
	public:
		Member1D(int id)
			: IdeaMember1D(id)
		{
		}

		property int CrossSectionId;

		property IIdeaCrossSection^ CrossSection
		{
			IIdeaCrossSection^ get() override
			{
				return Get<IIdeaCrossSection^>(CrossSectionId);
			}
		}
	};
}
