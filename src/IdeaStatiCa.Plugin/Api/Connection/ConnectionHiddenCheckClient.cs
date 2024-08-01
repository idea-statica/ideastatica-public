#if NET48

using IdeaRS.OpenModel.Connection;
using IdeaRS.OpenModel.CrossSection;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin
{
	public class ConnectionHiddenCheckClient : System.ServiceModel.ClientBase<IConnHiddenCheck>, IConnHiddenCheck
	{
		protected readonly IPluginLogger Logger;
		public static int HiddenCalculatorId { get; set; }

		static ConnectionHiddenCheckClient()
		{
			HiddenCalculatorId = -1;
		}

		public ConnectionHiddenCheckClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress, IPluginLogger logger) : base(binding, remoteAddress)
		{
			System.Diagnostics.Debug.Assert(logger != null);
			this.Logger = logger;
		}

		public ConnectionResultsData Calculate(string connectionId)
		{
			Logger.LogInformation($"ConnectionHiddenCheckClient.Calculate connectionId = '{connectionId}'");
			return Service.Calculate(connectionId);
		}

		public ConnectionResultsData CalculateBuckling(string connectionId)
		{
			Logger.LogInformation($"ConnectionHiddenCheckClient.Calculate connectionId = '{connectionId}'");
			return Service.CalculateBuckling(connectionId);
		}

		public void CloseProject()
		{
			Logger.LogInformation("ConnectionHiddenCheckClient.CloseProject");
			Service.CloseProject();
		}

		public ConProjectInfo GetProjectInfo()
		{
			Logger.LogInformation("ConnectionHiddenCheckClient.GetProjectInfo");
			return Service.GetProjectInfo();
		}

		public async Task<string> ApplySimpleTemplateAsync(string connectionId, string templateFilePath, ApplyConnTemplateSetting connTemplateSetting, int mainMember, List<int> attachedMembers)
		{
			Logger.LogInformation($"ConnectionHiddenCheckClient.ApplySimpleTemplate connectionId = '{connectionId}' templateFilePath ='{templateFilePath}'");
			return await Service.ApplySimpleTemplateAsync(connectionId, templateFilePath, connTemplateSetting, mainMember, attachedMembers);
		}

		public string ApplySimpleTemplate(string connectionId, string templateFilePath, ApplyConnTemplateSetting connTemplateSetting, int mainMember, List<int> attachedMembers)
		{
			return ApplySimpleTemplateAsync(connectionId, templateFilePath, connTemplateSetting, mainMember, attachedMembers).Result;
		}

		public void OpenProject(string ideaConFileName)
		{
			OpenProjectAsync(ideaConFileName)
				.Wait();
		}

		public async Task OpenProjectAsync(string ideaConFileName)
		{
			Logger.LogInformation($"ConnectionHiddenCheckClient.OpenProjectAsync ideaConFileName = '{ideaConFileName}'");
			await Service.OpenProjectAsync(ideaConFileName);
		}

		public void SaveAsProject(string newProjectFileName)
		{
			Logger.LogInformation($"ConnectionHiddenCheckClient.SaveAsProject newProjectFileName = '{newProjectFileName}'");
			Service.SaveAsProject(newProjectFileName);
		}

		public void Save()
		{
			Logger.LogInformation("ConnectionHiddenCheckClient.Save");
			Service.Save();
		}

		public async Task<string> ApplyTemplateAsync(string connectionId, string conTemplateFileName, ApplyConnTemplateSetting connTemplateSetting)
		{
			Logger.LogInformation($"ConnectionHiddenCheckClient.ApplyTemplate connectionId = '{connectionId}' conTemplateFileName ='{conTemplateFileName}'");
			return await Service.ApplyTemplateAsync(connectionId, conTemplateFileName, connTemplateSetting);
		}

		public string ApplyTemplate(string connectionId, string conTemplateFileName, ApplyConnTemplateSetting connTemplateSetting)
		{
			return ApplyTemplateAsync(connectionId, conTemplateFileName, connTemplateSetting).Result;
		}

		public string ExportToTemplate(string connectionId, string conTemplateFileName)
		{
			Logger.LogInformation($"ConnectionHiddenCheckClient.ExportToTemplate connectionId = '{connectionId}' conTemplateFileName ='{conTemplateFileName}'");
			return Service.ExportToTemplate(connectionId, conTemplateFileName);
		}

		public string GetConnectionCost(string connectionId)
		{
			Logger.LogInformation($"ConnectionHiddenCheckClient.GetConnectionCost connectionId = '{connectionId}'");
			return Service.GetConnectionCost(connectionId);
		}

		public ConnectionData GetConnectionModel(string connectionId)
		{
			Logger.LogInformation($"ConnectionHiddenCheckClient.GetConnectionModel connectionId = '{connectionId}'");
			return Service.GetConnectionModel(connectionId);
		}

		public string GetAllConnectionData(string connectionId)
		{
			Logger.LogInformation($"ConnectionHiddenCheckClient.GetAllConnectionData connectionId = '{connectionId}'");
			return Service.GetAllConnectionData(connectionId);
		}

		public string GetConnectionModelXML(string connectionId)
		{
			Logger.LogInformation($"ConnectionHiddenCheckClient.GetConnectionModelXML connectionId = '{connectionId}'");
			return Service.GetConnectionModelXML(connectionId);
		}

		public void UpdateConProjFromIOM(string iomXmlFileName, string iomResXmlFileName)
		{
			UpdateConProjFromIOMAsync(iomXmlFileName, iomResXmlFileName)
				.Wait();
		}

		public async Task UpdateConProjFromIOMAsync(string iomXmlFileName, string iomResXmlFileName)
		{
			Logger.LogInformation($"ConnectionHiddenCheckClient.UpdateConProjFromIOM iomXmlFileName ='{iomXmlFileName}'");
			await Service.UpdateConProjFromIOMAsync(iomXmlFileName, iomResXmlFileName);
		}

		public void CreateConProjFromIOM(string iomXmlFileName, string iomResXmlFileName, string newIdeaConFileName)
		{
			Logger.LogInformation($"ConnectionHiddenCheckClient.CreateConProjFromIOM iomXmlFileName ='{iomXmlFileName}'");
			Service.CreateConProjFromIOM(iomXmlFileName, iomResXmlFileName, newIdeaConFileName);
		}

		public void Cancel()
		{
			Logger.LogInformation("ConnectionHiddenCheckClient.Cancel");
			if (HiddenCalculatorId < 0)
			{
				return;
			}

			EventWaitHandle syncEvent;
			var cancelEventName = string.Format(Constants.ConCalculatorCancelEventFormat, HiddenCalculatorId);
			if (EventWaitHandle.TryOpenExisting(cancelEventName, out syncEvent))
			{
				syncEvent.Set();
				syncEvent.Dispose();
			}
		}

		public List<ProjectItem> GetMaterialsInProject()
		{
			Logger.LogInformation("ConnectionHiddenCheckClient.GetMaterialsInProject");
			return Service.GetMaterialsInProject();
		}

		public List<ProjectItem> GetCrossSectionsInProject()
		{
			Logger.LogInformation("ConnectionHiddenCheckClient.GetCrossSectionsInProject");
			return Service.GetCrossSectionsInProject();
		}

		public List<ProjectItem> GetBoltAssembliesInProject()
		{
			Logger.LogInformation("ConnectionHiddenCheckClient.GetBoltAssembliesInProject");
			return Service.GetBoltAssembliesInProject();
		}

		public async Task<int> AddBoltAssemblyAsync(string boltAssemblyName)
		{
			Logger.LogInformation($"ConnectionHiddenCheckClient.AddBoltAssembly boltAssemblyName = '{boltAssemblyName}'");
			return await Service.AddBoltAssemblyAsync(boltAssemblyName);
		}

		public int AddBoltAssembly(string boltAssemblyName)
		{
			return AddBoltAssemblyAsync(boltAssemblyName).Result;
		}

		public string GetParametersJSON(string connectionId)
		{
			Logger.LogInformation($"ConnectionHiddenCheckClient.GetParametersJSON connectionId = '{connectionId}'");
			return Service.GetParametersJSON(connectionId);
		}

		public async Task<string> ApplyParametersAsync(string connectionId, string parametersJSON)
		{
			Logger.LogInformation($"ConnectionHiddenCheckClient.ApplyParameters connectionId = '{connectionId}'");
			return await Service.ApplyParametersAsync(connectionId, parametersJSON);
		}

		public string ApplyParameters(string connectionId, string parametersJSON)
		{
			return ApplyParametersAsync(connectionId, parametersJSON).Result;
		}

		public string GetConnectionLoadingJSON(string connectionId)
		{
			Logger.LogInformation($"ConnectionHiddenCheckClient.GetConnectionLoadingJSON connectionId = '{connectionId}'");
			return Service.GetConnectionLoadingJSON(connectionId);
		}

		public async Task<string> UpdateLoadingFromJsonAsync(string connectionId, string loadingJSON)
		{
			Logger.LogInformation($"ConnectionHiddenCheckClient.UpdateLoadingFromJson connectionId = '{connectionId}'");
			return await Service.UpdateLoadingFromJsonAsync(connectionId, loadingJSON);
		}

		public string UpdateLoadingFromJson(string connectionId, string loadingJSON)
		{
			return UpdateLoadingFromJsonAsync(connectionId, loadingJSON).Result;
		}

		public string GetCheckResultsJSON(string connectionId)
		{
			Logger.LogInformation($"ConnectionHiddenCheckClient.GetCheckResultsJSON connectionId = '{connectionId}'");
			return Service.GetCheckResultsJSON(connectionId);
		}

		public string EvaluateExpression(string connectionId, string expression, string arumentsJSON)
		{
			Logger.LogInformation($"ConnectionHiddenCheckClient.EvaluateExpression connectionId = '{connectionId}'");
			return Service.EvaluateExpression(connectionId, expression, arumentsJSON);
		}

		public async Task<string> DeleteAllOperationsAsync(string connectionId)
		{
			Logger.LogInformation($"ConnectionHiddenCheckClient.DeleteAllOperations connectionId = '{connectionId}'");
			return await Service.DeleteAllOperationsAsync(connectionId);
		}

		public string DeleteAllOperations(string connectionId)
		{
			return DeleteAllOperationsAsync(connectionId).Result;
		}

		public string GetCodeSetupJSON()
		{
			Logger.LogInformation("ConnectionHiddenCheckClient.GetCodeSetupJSON");
			return Service.GetCodeSetupJSON();
		}

		public async Task<string> UpdateCodeSetupJsonAsync(string connectionSetupJSON)
		{
			Logger.LogInformation("ConnectionHiddenCheckClient.UpdateCodeSetupJSON");
			return await Service.UpdateCodeSetupJsonAsync(connectionSetupJSON);
		}

		public string UpdateCodeSetupJSON(string connectionSetupJSON)
		{
			return UpdateCodeSetupJsonAsync(connectionSetupJSON).Result;
		}

		public void SetCrossSectionMaterial(int crossSectionId, int materialId)
		{
			Logger.LogInformation($"ConnectionHiddenCheckClient.SetCrossSectionMaterial crossSectionId = {crossSectionId} materialId = {materialId}");
			Service.SetCrossSectionMaterial(crossSectionId, materialId);
		}

		public async Task SetMemberCrossSectionAsync(string connectionId, int memberId, int crossSectionId)
		{
			Logger.LogInformation($"ConnectionHiddenCheckClient.SetMemberCrossSection connectionId = '{connectionId}' memberId = {memberId} crossSectionId = {crossSectionId}");
			await Service.SetMemberCrossSectionAsync(connectionId, memberId, crossSectionId);
		}

		public void SetMemberCrossSection(string connectionId, int memberId, int crossSectionId)
		{
			Logger.LogInformation($"ConnectionHiddenCheckClient.SetMemberCrossSection connectionId = '{connectionId}' memberId = {memberId} crossSectionId = {crossSectionId}");
			SetMemberCrossSectionAsync(connectionId, memberId, crossSectionId).Wait();
		}

		public ReportResponse GenerateReport(string connectionId, ConnReportSettings settings)
		{
			return GenerateReportAsync(connectionId, settings)
				.Result;
		}

		/// <inheritdoc cref="IConnHiddenCheck.GenerateReportAsync(string, ConnReportSettings)"/>
		public async Task<ReportResponse> GenerateReportAsync(string connectionId, ConnReportSettings settings)
		{
			Logger.LogInformation($"ConnectionHiddenCheckClient.GenerateReport connectionId = '{connectionId}'");
			return await Service.GenerateReportAsync(connectionId, settings);
		}

		public void GenerateReportPdf(string connectionId, string filePath, ConnReportSettings settings)
		{
			GenerateReportPdfAsync(connectionId, filePath, settings)
				.Wait();
		}

		/// <inheritdoc cref="IConnHiddenCheck.GenerateReportPdfAsync(string, ConnReportSettings)"/>
		public async Task GenerateReportPdfAsync(string connectionId, string filePath, ConnReportSettings settings)
		{
			Logger.LogInformation($"ConnectionHiddenCheckClient.GenerateReport connectionId = '{connectionId}'");
			await Service.GenerateReportPdfAsync(connectionId, filePath, settings);
		}

		public void GenerateReportWord(string connectionId, string filePath, ConnReportSettings settings)
		{
			GenerateReportWordAsync(connectionId, filePath, settings)
				.Wait();
		}

		/// <inheritdoc cref="IConnHiddenCheck.GenerateReportWordAsync(string, string, ConnReportSettings)"/>
		public async Task GenerateReportWordAsync(string connectionId, string filePath, ConnReportSettings settings)
		{
			Logger.LogInformation($"ConnectionHiddenCheckClient.GenerateReport connectionId = '{connectionId}'");
			await Service.GenerateReportWordAsync(connectionId, filePath, settings);
		}

		public void OpenConnectionInApp(string connectionId)
		{
			OpenConnectionInAppAsync(connectionId)
				.Wait();
		}

		/// <inheritdoc cref="IConnHiddenCheck.OpenConnectionInAppAsync(string)"/>
		public async Task OpenConnectionInAppAsync(string connectionId)
		{
			Logger.LogInformation($"ConnectionHiddenCheckClient.OpenConnectionInApp connectionId = '{connectionId}'");
			await Service.OpenConnectionInAppAsync(connectionId);
		}

		protected IConnHiddenCheck Service => base.Channel;
	}
}
#endif