using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdeaRS.OpenModel.Concrete.CheckResult;
using IdeaRS.OpenModel.Message;
using IdeaRS.OpenModel;
using System.Xml;

namespace IdeaStatiCa.Plugin.Api.Rcs
{
	public class ProjectResult
	{
		public List<SectionConcreteCheckResult> Sections { get; set; } = new List<SectionConcreteCheckResult>();

		public List<NonConformityIssue> Issues { get; set; } = new List<NonConformityIssue>();
	}

	public class RcsProjectInfo
	{
		// Direct path to idea project
		public string IdeaProjectPath { get; set; }

		// Direct path to open model project
		public string IdeaOpenModelProjectPath { get; set; }
		public string OpenMessagesPath { get; set; }
		public string ProjectName { get; set; }

		public IEnumerable<int> Sections { get; set; } = new List<int>();
		public IEnumerable<Guid> NonConformities { get; set; } = new List<Guid>();

		public TimeSpan CalculationTimeout { get; set; }

		public override string ToString() => $"Project path: '{IdeaProjectPath}', Project name: '{ProjectName}'";
	}
	public interface IRcsController : IDisposable
	{
		bool OpenIdeaProject(string ideaProjectPath);
		bool OpenIdeaProjectFromIdeaOpenModel(string ideaOpenModelProjectPath, string projectName, string ideaOpenMessagesPath);
		OpenMessages OpenIdeaProjectFromXMLOpenModel(XmlReader openModelXML, string projectName);
		OpenMessages OpenIdeaProjectFromIdeaOpenModel(OpenModel ideaOpenModel, string projectName);

		int[] GetProjectSections();


		bool Calculate(IEnumerable<int> sections);
		IEnumerable<SectionConcreteCheckResult> GetResultOnSections(params int[] sections);
		IEnumerable<NonConformityIssue> GetNonConformityIssues(params Guid[] issues);
	}
}
