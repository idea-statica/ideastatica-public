using IdeaRS.OpenModel.Connection;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin
{
	/// <summary>
	/// The identifier of the generated report
	/// </summary>
	public class ReportResponse
	{
		/// <summary>
		/// Port of grpc server which can provide a report
		/// </summary>
		[DataMember]
		public int Port { get; set; }

		/// <summary>
		/// Unique report identifier
		/// </summary>
		[DataMember]
		public string ReportId { get; set; }
	}

	/// <summary>
	/// Provides seamlessly data about connections or allows to run hidden calculation of a connection
	/// </summary>
	
	public interface IConnHiddenCheck
	{
		/// <summary>
		/// Open idea project in the service
		/// </summary>
		/// <param name="ideaConProject">Idea Connection project.</param>
		
		void OpenProject(string ideaConProject);

		/// <summary>
		/// Save the current data in file <paramref name="newProjectFileName"/>
		/// </summary>
		/// <param name="newProjectFileName">File name of the new idea connection project</param>
		
		void SaveAsProject(string newProjectFileName);

		/// <summary>
		/// Saves the current data
		/// </summary>
		
		void Save();

		/// <summary>
		/// Apply the selected template in file <paramref name="conTemplateFileName"/> on connection <paramref name="connectionId"/>
		/// </summary>
		/// <param name="connectionId">Identifier of the connection in the project, empty guid means the first connection in the project</param>
		/// <param name="conTemplateFileName">contemp filename including connection template</param>
		/// <param name="connTemplateSetting">Additional setting for application of the template.</param>
		/// <returns>returns 'OK' if success otherwise an error message</returns>
		
		Task<string> ApplyTemplateAsynchronous(string connectionId, string conTemplateFileName, ApplyConnTemplateSetting connTemplateSetting);

		/// <summary>
		/// Apply the selected template in file <paramref name="conTemplateFileName"/> on connection <paramref name="connectionId"/>
		/// </summary>
		/// <param name="connectionId">Identifier of the connection in the project, empty guid means the first connection in the project</param>
		/// <param name="conTemplateFileName">contemp filename including connection template</param>
		/// <param name="connTemplateSetting">Additional setting for application of the template.</param>
		/// <returns>returns 'OK' if success otherwise an error message</returns>
		/// <remarks>This synchronous method internally invokes the asynchronous code, which might cause application freezes and failures
		/// when run on the UI thread. When you attempt to run this method on the UI thread, run the asynchronous version of this method instead
		/// using the await keyword.</remarks>
		
		string ApplyTemplate(string connectionId, string conTemplateFileName, ApplyConnTemplateSetting connTemplateSetting);

		/// <summary>
		/// Apply the simple connectionsimple template from file <paramref name="templateFilePath"/> on connection <paramref name="connectionId"/>
		/// </summary>
		/// <param name="connectionId">The id of the connection on which templete will be applied</param>
		/// <param name="templateFilePath">The path to the connection teplate</param>
		/// <param name="connTemplateSetting">The additional settings - e.g. default bolts</param>
		/// <param name="mainMember">Main (supporting member)</param>
		/// <param name="attachedMembers">The list of members which are supported by <paramref name="mainMember"/></param>
		/// <returns>Returns 'Ok' in case of the success otherwise 'Fail'</returns>
		
		Task<string> ApplySimpleTemplateAsynchronous(string connectionId, string templateFilePath, ApplyConnTemplateSetting connTemplateSetting, int mainMember, List<int> attachedMembers);

		/// <summary>
		/// Apply the simple connectionsimple template from file <paramref name="templateFilePath"/> on connection <paramref name="connectionId"/>
		/// </summary>
		/// <param name="connectionId">The id of the connection on which templete will be applied</param>
		/// <param name="templateFilePath">The path to the connection teplate</param>
		/// <param name="connTemplateSetting">The additional settings - e.g. default bolts</param>
		/// <param name="mainMember">Main (supporting member)</param>
		/// <param name="attachedMembers">The list of members which are supported by <paramref name="mainMember"/></param>
		/// <returns>Returns 'Ok' in case of the success otherwise 'Fail'</returns>
		/// <remarks>This synchronous method internally invokes the asynchronous code, which might cause application freezes and failures
		/// when run on the UI thread. When you attempt to run this method on the UI thread, run the asynchronous version of this method instead
		/// using the await keyword.</remarks>
		
		string ApplySimpleTemplate(string connectionId, string templateFilePath, ApplyConnTemplateSetting connTemplateSetting, int mainMember, List<int> attachedMembers);

		/// <summary>
		/// Get the geometry of the <paramref name="connectionId"/>
		/// </summary>
		/// <param name="connectionId">Identifier of the required connection</param>
		/// <returns>Geometry of the connection in the Xml format</returns>
		
		string GetConnectionModelXML(string connectionId);

		/// <summary>
		/// Export the manufacture sequence of <paramref name="connectionId"/> as a template and save it in <paramref name="conTemplateFileName"/> (.contemp file)
		/// </summary>
		/// <param name="connectionId">>Identifier of the connection in the project, empty guid means the first connection in the project</param>
		/// <param name="conTemplateFileName">The file name of the output file</param>
		/// <returns>returns 'OK' if success otherwise an error message</returns>
		
		string ExportToTemplate(string connectionId, string conTemplateFileName);

		/// <summary>
		/// Gets details about the open project and its connections
		/// </summary>
		/// <returns></returns>
		
		ConProjectInfo GetProjectInfo();

		/// <summary>
		/// Cakculate connection given by <paramref name="connectionId"/>
		/// </summary>
		/// <param name="connectionId">The identifier of the required connection</param>
		/// <returns>Connection check results</returns>
		
		ConnectionResultsData Calculate(string connectionId);

		
		ConnectionResultsData CalculateBuckling(string connectionId);

		/// <summary>
		/// Gets connection price given by <paramref name="connectionId"/>
		/// </summary>
		/// <param name="connectionId">The identifier of the required connection</param>
		/// <returns>Connection price results</returns>
		
		string GetConnectionCost(string connectionId);
		/// <summary>
		/// Get the geometry of the <paramref name="connectionId"/>
		/// </summary>
		/// <param name="connectionId">Identifier of the required connection</param>
		/// <returns>Geometry of the connection in the IOM format</returns>
		
		IdeaRS.OpenModel.Connection.ConnectionData GetConnectionModel(string connectionId);

		/// <summary>
		/// Get structural data and corresponding results of FE analysi for <paramref name="connectionId"/>
		/// </summary>
		/// <param name="connectionId">Identifier of the required connection</param>
		/// <returns>XML string which prepresents the instance of of IdeaRS.OpenModel.OpenModelContainer (stuctural data and results of FE analysis)</returns>
		
		string GetAllConnectionData(string connectionId);

		/// <summary>
		/// Evaluate expression on connection model <paramref name="connectionId"/>
		/// </summary>
		/// <param name="connectionId">Id of the connection in the open idea connection project</param>
		/// <param name="expression">User expression to be evaluated</param>
		/// <param name="arumentsJSON">Optional agruments in json format (not used now)</param>
		/// <returns>In case of a success it is the JSON string representing the result of the query. String 'null' if nothing is found. String 'error' in case of any other unspecified error.</returns>
		
		string EvaluateExpression(string connectionId, string expression, string arumentsJSON);

		/// <summary>
		/// Update opened Idea connection project from given <paramref name="iomXmlFileName"/>, <paramref name="iomResXmlFileName"/>
		/// </summary>
		/// <param name="iomXmlFileName">Filename of a given IOM xml file</param>
		/// <param name="iomResXmlFileName">Filename of a given IOM Result xml file</param>
		
		void UpdateConProjFromIOM(string iomXmlFileName, string iomResXmlFileName);

		/// <summary>
		/// Creates Idea connection project from given <paramref name="iomXmlFileName"/>, <paramref name="iomResXmlFileName"/> and projects saves into the <paramref name="newIdeaConFileName"/>
		/// </summary>
		/// <param name="iomXmlFileName">Filename of a given IOM xml file</param>
		/// <param name="iomResXmlFileName">Filename of a given IOM Result xml file</param>
		/// <param name="newIdeaConFileName">File name of idea connection project where generated project will be saved</param>
		
		void CreateConProjFromIOM(string iomXmlFileName, string iomResXmlFileName, string newIdeaConFileName);

		/// <summary>
		/// Close project which is open in the service
		/// </summary>
		
		void CloseProject();

		/// <summary>
		/// Cancel current calcullation
		/// </summary>
		
		void Cancel();

		/// <summary>
		/// Get all materials in the currently open project
		/// </summary>
		/// <returns>Materials in the project</returns>
		
		List<ProjectItem> GetMaterialsInProject();

		/// <summary>
		/// Get all cross-sections in the currently open project
		/// </summary>
		/// <returns>Cross-sections in the project</returns>
		
		List<ProjectItem> GetCrossSectionsInProject();

		/// <summary>
		/// Get all bolt assemblies in the currently open project
		/// </summary>
		/// <returns>Bolt assemblies in the project</returns>
		
		List<ProjectItem> GetBoltAssembliesInProject();

		/// <summary>
		/// Add the new bolt assembly. Its type is defined by its name (e.g. 'M12 4.6')
		/// </summary>
		/// <param name="boltAssemblyName"></param>
		/// <returns></returns>
		
		Task<int> AddBoltAssemblyAsynchronous(string boltAssemblyName);

		/// <summary>
		/// Add the new bolt assembly. Its type is defined by its name (e.g. 'M12 4.6')
		/// </summary>
		/// <param name="boltAssemblyName"></param>
		/// <returns></returns>
		/// <remarks>This synchronous method internally invokes the asynchronous code, which might cause application freezes and failures
		/// when run on the UI thread. When you attempt to run this method on the UI thread, run the asynchronous version of this method instead
		/// using the await keyword.</remarks>
		
		int AddBoltAssembly(string boltAssemblyName);

		/// <summary>
		/// Get list of parameters in JSON format for <paramref name="connectionId"/>
		/// <see href="https://idea-statica.github.io/iom/iom-api/latest/html/N_IdeaRS_OpenModel_Parameters.htm">IOM Parameters</see>
		/// </summary>
		/// <param name="connectionId">Id of the parameter</param>
		/// <returns></returns>
		
		string GetParametersJSON(string connectionId);

		/// <summary>
		/// Get code setup in JSON format 
		/// <see href="https://idea-statica.github.io/iom/iom-api/latest/html/Properties_T_IdeaRS_OpenModel_ConnectionSetup.htm">IOM connection setup</see>
		/// </summary>
		/// <returns></returns>
		
		string GetCodeSetupJSON();

		/// <summary>
		/// Apply <paramref name="parametersJSON"/> on connection<paramref name="connectionId"/>
		/// <see href="https://idea-statica.github.io/iom/iom-api/latest/html/N_IdeaRS_OpenModel_Parameters.htm">IOM Parameters</see>
		/// </summary>
		/// <param name="connectionId">Id of the parameter</param>
		/// <param name="parametersJSON">JSON string including parameters</param>
		/// <returns>True if success | False if apply of parameters failed OR model contain some nonconformity</returns>
		
		Task<string> ApplyParametersAsynchronous(string connectionId, string parametersJSON);

		/// <summary>
		/// Apply <paramref name="parametersJSON"/> on connection<paramref name="connectionId"/>
		/// <see href="https://idea-statica.github.io/iom/iom-api/latest/html/N_IdeaRS_OpenModel_Parameters.htm">IOM Parameters</see>
		/// </summary>
		/// <param name="connectionId">Id of the parameter</param>
		/// <param name="parametersJSON">JSON string including parameters</param>
		/// <returns>True if success | False if apply of parameters failed OR model contain some nonconformity</returns>
		/// <remarks>This synchronous method internally invokes the asynchronous code, which might cause application freezes and failures
		/// when run on the UI thread. When you attempt to run this method on the UI thread, run the asynchronous version of this method instead
		/// using the await keyword.</remarks>
		
		string ApplyParameters(string connectionId, string parametersJSON);

		/// <summary>
		/// Get loading for connection <paramref name="connectionId"/> (List of CalcCaseData)
		/// </summary>
		/// <param name="connectionId">Id of the parameter</param>
		/// <returns></returns>
		
		string GetConnectionLoadingJSON(string connectionId);

		/// <summary>
		/// Set loading for the connection
		/// </summary>
		/// <param name="connectionId">Id of the connection</param>
		/// <param name="loadingJSON">JSON including list of CalcCaseData</param>
		/// <returns></returns>
		
		Task<string> UpdateLoadingFromJsonAsynchronous(string connectionId, string loadingJSON);

		/// <summary>
		/// Set loading for the connection
		/// </summary>
		/// <param name="connectionId">Id of the connection</param>
		/// <param name="loadingJSON">JSON including list of CalcCaseData</param>
		/// <returns></returns>
		/// <remarks>This synchronous method internally invokes the asynchronous code, which might cause application freezes and failures
		/// when run on the UI thread. When you attempt to run this method on the UI thread, run the asynchronous version of this method instead
		/// using the await keyword.</remarks>
		
		string UpdateLoadingFromJson(string connectionId, string loadingJSON);

		/// <summary>
		/// Set codesetup for the connection
		/// </summary>
		/// <param name="connectionSetupJSON">connection setup in JSON format</param>
		/// <returns></returns>
		
		Task<string> UpdateCodeSetupJsonAsynchronous(string connectionSetupJSON);

		/// <summary>
		/// Set codesetup for the connection
		/// </summary>
		/// <param name="connectionSetupJSON">connection setup in JSON format</param>
		/// <returns></returns>
		
		string UpdateCodeSetupJSON(string connectionSetupJSON);

		/// <summary>
		/// Get the details results of the connection <paramref name="connectionId"/>
		/// </summary>
		/// <param name="connectionId">Id of the connection</param>
		/// <returns>Json which represents </returns>
		
		string GetCheckResultsJSON(string connectionId);

		/// <summary>
		/// Delete all manufacturing operations in <paramref name="connectionId"/>
		/// </summary>
		/// <param name="connectionId">Id of the connection</param>
		/// <returns>Returns 'Ok' in case of the success otherwise 'Fail'</returns>
		
		Task<string> DeleteAllOperationsAsynchronous(string connectionId);

		/// <summary>
		/// Delete all manufacturing operations in <paramref name="connectionId"/>
		/// </summary>
		/// <param name="connectionId">Id of the connection</param>
		/// <returns>Returns 'Ok' in case of the success otherwise 'Fail'</returns>
		/// <remarks>This synchronous method internally invokes the asynchronous code, which might cause application freezes and failures
		/// when run on the UI thread. When you attempt to run this method on the UI thread, run the asynchronous version of this method instead
		/// using the await keyword.</remarks>
		
		string DeleteAllOperations(string connectionId);

		/// <summary>
		/// Change the material of a cross-section currently available in the Project
		/// </summary>
		/// <param name="crossSectionId">Id of the cross-section</param>
		/// <param name="materialId">Id of the material</param>
		
		void SetCrossSectionMaterial(int crossSectionId, int materialId);

		/// <summary>
		/// Change the cross-section of a member in a connection in the Project through
		/// </summary>
		/// <param name="connectionId">Id of the connection</param>
		/// <param name="memberId">Id of the member</param>
		/// <param name="crossSectionId">Id of the cross-section</param>
		
		Task SetMemberCrossSectionAsynchronous(string connectionId, int memberId, int crossSectionId);

		/// <summary>
		/// Change the cross-section of a member in a connection in the Project through
		/// </summary>
		/// <param name="connectionId">Id of the connection</param>
		/// <param name="memberId">Id of the member</param>
		/// <param name="crossSectionId">Id of the cross-section</param>
		/// <remarks>This synchronous method internally invokes the asynchronous code, which might cause application freezes and failures
		/// when run on the UI thread. When you attempt to run this method on the UI thread, run the asynchronous version of this method instead
		/// using the await keyword.</remarks>
		
		void SetMemberCrossSection(string connectionId, int memberId, int crossSectionId);

		/// <summary>
		/// Generate a report for connection <paramref name="connectionId"/>
		/// </summary>
		/// <param name="connectionId">The unique identifier of the requested connection</param>
		/// <param name="settings">Report settings</param>
		/// <returns>The identifier of the generated report. It will be used in <see cref="IdeaStatiCa.Plugin.Grpc.GrpcBlobStorageClient"/> requests</returns>
		
		ReportResponse GenerateReport(string connectionId, ConnReportSettings settings);

		/// <summary>
		/// Generate a pdf report for connection <paramref name="connectionId"/>
		/// </summary>
		/// <param name="connectionId">The unique identifier of the requested connection</param>
		/// <param name="settings">Report settings</param>
		/// <param name="filePath">File path of the exported pdf report </param>
		
		void GenerateReportPdf(string connectionId, string filePath, ConnReportSettings settings);

		/// <summary>
		/// Generate a word report for connection <paramref name="connectionId"/>
		/// </summary>
		/// <param name="connectionId">The unique identifier of the requested connection</param>
		/// <param name="settings">Report settings</param>
		/// <param name="filePath">File path of the exported word report </param>
		
		void GenerateReportWord(string connectionId, string filePath, ConnReportSettings settings);

		/// <summary>
		/// Open and select the connection <paramref name="connectionId"/> in an application
		/// </summary>
		/// <param name="connectionId">The unique identifier of the requested connection</param>
		
		void OpenConnectionInApp(string connectionId);
	}
}