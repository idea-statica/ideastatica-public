#if NET48

using IdeaRS.OpenModel.Connection;
using System.Collections.Generic;
using System.Threading;

namespace IdeaStatiCa.Plugin
{
	public class ConnectionHiddenCheckClient : System.ServiceModel.ClientBase<IConnHiddenCheck>, IConnHiddenCheck
	{
		public static int HiddenCalculatorId { get; set; }

		static ConnectionHiddenCheckClient()
		{
			HiddenCalculatorId = -1;
		}

		public ConnectionHiddenCheckClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : base(binding, remoteAddress)
		{
		}

		public ConnectionResultsData Calculate(string connectionId)
		{
			return Service.Calculate(connectionId);
		}

		public void CloseProject()
		{
			Service.CloseProject();
		}

		public ConProjectInfo GetProjectInfo()
		{
			return Service.GetProjectInfo();
		}

		public string ApplySimpleTemplate(string connectionId, string templateFilePath, ApplyConnTemplateSetting connTemplateSetting, int mainMember, List<int> attachedMembers)
		{
			return Service.ApplySimpleTemplate(connectionId, templateFilePath, connTemplateSetting, mainMember, attachedMembers);
		}

		public void OpenProject(string ideaConFileName)
		{
			Service.OpenProject(ideaConFileName);
		}

		public void SaveAsProject(string newProjectFileName)
		{
			Service.SaveAsProject(newProjectFileName);
		}
		public void Save()
		{
			Service.Save();
		}

		public string ApplyTemplate(string connectionId, string conTemplateFileName, ApplyConnTemplateSetting connTemplateSetting)
		{
			return Service.ApplyTemplate(connectionId, conTemplateFileName, connTemplateSetting);
		}

		public string ExportToTemplate(string connectionId, string conTemplateFileName)
		{
			return Service.ExportToTemplate(connectionId, conTemplateFileName);
		}
		public string GetConnectionCost(string connectionId)
		{
			return Service.GetConnectionCost(connectionId);
		}

		public ConnectionData GetConnectionModel(string connectionId)
		{
			return Service.GetConnectionModel(connectionId);
		}

		public string GetAllConnectionData(string connectionId)
		{
			return Service.GetAllConnectionData(connectionId);
		}

		public string GetConnectionModelXML(string connectionId)
		{
			return Service.GetConnectionModelXML(connectionId);
		}
		public void UpdateConProjFromIOM(string iomXmlFileName, string iomResXmlFileName)
		{
			Service.UpdateConProjFromIOM(iomXmlFileName, iomResXmlFileName);
		}

		public void CreateConProjFromIOM(string iomXmlFileName, string iomResXmlFileName, string newIdeaConFileName)
		{
			Service.CreateConProjFromIOM(iomXmlFileName, iomResXmlFileName, newIdeaConFileName);
		}

		public void Cancel()
		{
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
			return Service.GetMaterialsInProject();
		}

		public List<ProjectItem> GetCrossSectionsInProject()
		{
			return Service.GetCrossSectionsInProject();
		}

		public List<ProjectItem> GetBoltAssembliesInProject()
		{
			return Service.GetBoltAssembliesInProject();
		}

		public int AddBoltAssembly(string boltAssemblyName)
		{
			return Service.AddBoltAssembly(boltAssemblyName);
		}

		public string GetParametersJSON(string connectionId)
		{
			return Service.GetParametersJSON(connectionId);
		}

		public string ApplyParameters(string connectionId, string parametersJSON)
		{
			return Service.ApplyParameters(connectionId, parametersJSON);
		}

		public string GetConnectionLoadingJSON(string connectionId)
		{
			return Service.GetConnectionLoadingJSON(connectionId);
		}

		public string UpdateLoadingFromJson(string connectionId, string loadingJSON)
		{
			return Service.UpdateLoadingFromJson(connectionId, loadingJSON);
		}

		public string GetCheckResultsJSON(string connectionId)
		{
			return Service.GetCheckResultsJSON(connectionId);
		}

		public string EvaluateExpression(string connectionId, string expression, string arumentsJSON)
		{
			return Service.EvaluateExpression(connectionId, expression, arumentsJSON);
		}

		public string DeleteAllOperations(string connectionId)
		{
			return Service.DeleteAllOperations(connectionId);
		}

		public string GetCodeSetupJSON()
		{
			return Service.GetCodeSetupJSON();
		}

		public string UpdateCodeSetupJSON(string connectionSetupJSON)
		{
			return Service.UpdateCodeSetupJSON(connectionSetupJSON);
		}

		public void SetCrossSectionMaterial(int crossSectionId, int materialId)
		{
			Service.SetCrossSectionMaterial(crossSectionId, materialId);
		}

		public void SetMemberCrossSection(string connectionId, int memberId, int crossSectionId)
		{
			Service.SetMemberCrossSection(connectionId, memberId, crossSectionId);
		}

		protected IConnHiddenCheck Service => base.Channel;
	}
}
#endif