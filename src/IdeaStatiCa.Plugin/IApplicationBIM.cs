using System.Collections.Generic;
using System.Threading.Tasks;
using System.ServiceModel;

namespace IdeaStatiCa.Plugin
{
	/// <summary>
	/// Abstraction of a FE application which provides data to Idea StatiCa
	/// </summary>
	[ServiceContract]
	public interface IApplicationBIM
	{
		[OperationContract]
		List<BIMItemId> GetActiveSelection();

		[OperationContract]
		string GetActiveSelectionModelXML(IdeaRS.OpenModel.CountryCode countryCode, RequestedItemsType requestedType);
		Task<string> GetActiveSelectionModelXMLAsync(IdeaRS.OpenModel.CountryCode countryCode, RequestedItemsType requestedType);

		[OperationContract]
		string GetApplicationName();

		/// <summary>
		/// Returns a xml string representing a list of BIM models for requested sequence of given <paramref name="items"/> - groups.
		/// Each group in the list of <paramref name="items"/> represents designed item such as connection or member and is defined by the id, type and items belonging to the group.
		/// The item can also has id = -1, which means, that items (members or nodes typically) in this group doesn't belong to any design item (connection or member).
		/// </summary>
		/// <param name="countryCode">The standard that will match the imported models.</param>
		/// <param name="items">The sequence of items, for which the BIM model is required.</param>
		/// <returns>A xml string representing a list of BIM models for requested <paramref name="items"/>.</returns>
		[OperationContract]
		string GetModelForSelectionXML(IdeaRS.OpenModel.CountryCode countryCode, List<BIMItemsGroup> items);

		Task<string> GetModelForSelectionXMLAsync(IdeaRS.OpenModel.CountryCode countryCode, List<BIMItemsGroup> items);

		[OperationContract]
		bool IsCAD();

		[OperationContract]
		Task SelectAsync(List<BIMItemId> items);
		bool IsDataUpToDate();
	}
}
