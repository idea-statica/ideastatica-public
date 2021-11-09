using IdeaRS.OpenModel;
using IdeaStatiCa.Plugin;

namespace IdeaRstabPlugin
{
	internal interface IImportSession
	{
		CountryCode CountryCode { get; }

		//bool IsLCSOrientedUpwards { get; }

		bool IsGCSOrientedUpwards { get; }

		RequestedItemsType RequestedItemsType { get; }
	}
}