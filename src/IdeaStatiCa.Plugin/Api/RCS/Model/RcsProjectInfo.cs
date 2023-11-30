using System;
using System.Collections.Generic;
using IdeaRS.OpenModel;

namespace IdeaStatiCa.Plugin.Api.RCS.Model
{
	public class RcsProjectInfo
	{
		// Direct path to idea project
		public string IdeaProjectPath { get; set; }

		// Direct path to open model project
		public string IdeaOpenModelProjectPath { get; set; }
		public string OpenMessagesPath { get; set; }
		public string ProjectName { get; set; }

		// Option for XML Open model object
		public OpenModel OpenModel { get; set; }
		public override string ToString() => $"Project path: '{IdeaProjectPath}', Project name: '{ProjectName}'";
	}

	public class RcsCalculationParameters
	{
		public IEnumerable<int> Sections { get; set; } = new List<int>();
		public IEnumerable<Guid> NonConformities { get; set; } = new List<Guid>();
	}
}
