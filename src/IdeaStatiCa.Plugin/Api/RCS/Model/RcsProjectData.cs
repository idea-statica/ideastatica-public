using System;

namespace IdeaStatiCa.Plugin.Api.RCS.Model
{
	public class RcsProjectData
	{
		public string ProjectName { get; set; }
		public DateTime DateOfCreate { get; set; }
		public string Description { get; set; }
		public string Author { get; set; }
		public string Code { get; set; }
		public string ProjectNo { get; set; }
		public string TypeBridge { get; set; }
		public string TypeOfStructure { get; set; }
		public bool FireResistanceCheck { get; set; }
	}
}
