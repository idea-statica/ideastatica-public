using IdeaRS.OpenModel;
using IdeaStatiCa.Plugin;

namespace IdeaStatiCa.RamToIdea
{
	internal interface IImportSession
	{
		CountryCode CountryCode { get; }

		//bool IsLCSOrientedUpwards { get; }

		bool IsGCSOrientedUpwards { get; }

		RequestedItemsType RequestedItemsType { get; }
	}
}
