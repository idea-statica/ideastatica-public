using IdeaStatiCa.Api.Connection.Model;
using System;
using System.Collections.Generic;

namespace ConApiWpfClientApp.Models
{
	public class ConnectionLibraryModel
	{
		public ConnectionLibraryModel()
		{
			this.SearchParameters = new ConConnectionLibrarySearchParameters();
			this.ProposedDesignItems = new List<ConDesignItem>();
		}

		public Guid ProjectId { get; set; }
		public int ConnectionId { get; set; }

		public ConConnectionLibrarySearchParameters SearchParameters { get; set; }

		public List<ConDesignItem> ProposedDesignItems { get; set; }

		public string? SelectedTemplateXml { get; set; }

		public byte[]? PictureData { get; set; }
	}
}
