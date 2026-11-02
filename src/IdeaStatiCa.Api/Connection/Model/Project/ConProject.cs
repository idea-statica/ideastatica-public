using System;
using System.Collections.Generic;

namespace IdeaStatiCa.Api.Connection.Model
{
	public class ConProject
	{
		/// <summary>
		/// Thw unique identifier of an open connection project.
		/// It is assigned by Rest API ahen a project is uploaded.
		/// </summary>
		public Guid ProjectId { get; set; }

		public ConProjectData ProjectInfo { get; set; }

		public List<ConConnection> Connections { get; set; }
	}
}