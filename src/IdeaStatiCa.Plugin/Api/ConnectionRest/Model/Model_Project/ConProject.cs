using IdeaStatiCa.Plugin.Api.ConnectionRest.Model.Model_Connection;
using System;
using System.Collections.Generic;

namespace IdeaStatiCa.Plugin.Api.ConnectionRest.Model.Model_Project
{
	public class ConProject
	{
		public Guid ProjectId { get; set; }

		public ConProjectData ProjectInfo { get; set; }

		public List<ConConnection> Connections { get; set; }
	}
}