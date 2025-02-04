using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;

namespace IdeaStatiCa.Plugin
{
	/// <summary>
	/// Identification of an item in IDEA StatiCa MPRL (material and product range library)
	/// </summary>
	[DataContract]
	public class LibraryItem
	{
		[DataMember]
		public string Group { get; set; }

		[DataMember]
		public string Identifier { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string GroupId { get; set; }
	}

	/// <summary>
	/// Identification of an item in the Idea project
	/// </summary>
	[DataContract]
	public class ProjectItem
	{
		/// <summary>
		/// Id of the item in the idea project
		/// </summary>
		[DataMember]
		public int Identifier { get; set; }

		/// <summary>
		/// Name of the item in the idea project
		/// </summary>
		[DataMember]
		public string Name { get; set; }
	}

	/// <summary>
	/// Identification of an crossection item in the Idea project
	/// </summary>
	[DataContract]
	public class CrossSectionProjectItem : ProjectItem
	{
		/// <summary>
		/// Id of the material item in the idea project
		/// </summary>
		[DataMember]
		public int MaterialIdentifier { get; set; }

		/// <summary>
		/// Name of the material item in the idea project
		/// </summary>
		[DataMember]
		public string MaterialName { get; set; }
	}


	
	public interface IIdeaStaticaApp: IProgressMessaging
	{
		/// <summary>
		/// Get all cross-sections from IDEA StatiCa MPRL (material and product range library) which belongs to <paramref name="countryCode"/>
		/// </summary>
		/// <param name="countryCode">Country code filter</param>
		/// <returns>Cross-sections in the MPRL</returns>
		
		List<LibraryItem> GetCssInMPRL(IdeaRS.OpenModel.CountryCode countryCode);

		/// <summary>
		/// Get all materials from IDEA StatiCa MPRL (material and product range library) which belongs to <paramref name="countryCode"/>
		/// </summary>
		/// <param name="countryCode">Country code filter</param>
		/// <returns>Materials in the MPRL</returns>
		
		List<LibraryItem> GetMaterialsInMPRL(IdeaRS.OpenModel.CountryCode countryCode);

		/// <summary>
		/// Get all cross-sections in the currently open project
		/// </summary>
		/// <returns>Cross-sections in the project</returns>
		
		List<ProjectItem> GetCssInProject();

		/// <summary>
		/// Get all cross-sections in the currently open project
		/// </summary>
		/// <returns>Cross-sections with assigned material in the project</returns>
		
		List<CrossSectionProjectItem> GetCssInProjectV2();

		/// <summary>
		/// Get all materials in the currently open project
		/// </summary>
		/// <returns>Materials in the project</returns>
		
		List<ProjectItem> GetMaterialsInProject();

		/// <summary>
		/// Get connection model in IOM format
		/// </summary>
		/// <param name="connectionId">The ID of the connection in the project</param>
		/// <exception cref="System.Exception">Exception is thrown if operation fails or no data are provided by the service</exception>
		/// <returns>Connection model</returns>
		IdeaRS.OpenModel.Connection.ConnectionData GetConnectionModel(int connectionId);

		/// <summary>
		/// Get structural data and corresponding results of FE analysis for <paramref name="connectionId"/>
		/// </summary>
		/// <param name="connectionId">Identifier of the required connection</param>
		/// <exception cref="System.Exception">Exception is thrown if operation fails or no data are provided by the service</exception>
		/// <returns>XML string which prepresents the instance of of IdeaRS.OpenModel.OpenModelContainer (stuctural data and results of FE analysis)</returns>
		
		string GetAllConnectionData(int connectionId);
	}
}