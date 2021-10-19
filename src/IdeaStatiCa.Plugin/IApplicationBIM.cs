using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin
{
	public enum RequestedItemsType
	{
		Connections,
		Substructure,
		SingleConnection,

		// Items, that are not in any design item (connection, substructure), but should be synchronized
		Unassigned,
	}

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

		[OperationContract]
		string GetApplicationName();

		[OperationContract]
		string GetModelForSelectionXML(IdeaRS.OpenModel.CountryCode countryCode, List<BIMItemsGroup> items);

		[OperationContract]
		bool IsCAD();

		[OperationContract]
		Task SelectAsync(List<BIMItemId> items);
	}
}