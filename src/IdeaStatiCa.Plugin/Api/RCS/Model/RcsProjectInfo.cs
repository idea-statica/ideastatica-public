using System;
using System.Collections.Generic;
using System.IO;
using IdeaRS.OpenModel;

namespace IdeaStatiCa.Plugin.Api.RCS.Model
{
	public class RcsProjectInfo
	{
		// Direct path to idea project
		public string IdeaProjectPath { get; set; }

		// Direct path to open model project
		public string IdeaOpenModelProjectPath { get; set; }
		public string ProjectName { get; set; }

		// Option for XML Open model object
		public OpenModel OpenModel { get; set; }

		// Option for MemoryStream
		public MemoryStream MemoryStream { get; set; }
		public string FileExtension { get; set; }

		public override string ToString() => $"Project path: '{IdeaProjectPath}', Project name: '{ProjectName}'";
	}

	public class RcsCalculationParameters
	{
		public IEnumerable<int> Sections { get; set; } = new List<int>();
	}
}
