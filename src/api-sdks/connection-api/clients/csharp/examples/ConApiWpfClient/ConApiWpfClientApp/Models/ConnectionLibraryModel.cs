using IdeaStatiCa.Api.Connection.Model;
using System;
using System.Collections.Generic;

namespace ConApiWpfClientApp.Models
{
	/// <summary>
	/// Model representing the state of a connection library search and selection,
	/// including search parameters, proposed design items, and the selected template.
	/// </summary>
	public class ConnectionLibraryModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ConnectionLibraryModel"/> class
		/// with default search parameters and an empty list of proposed design items.
		/// </summary>
		public ConnectionLibraryModel()
		{
			this.SearchParameters = new ConConnectionLibrarySearchParameters();
			this.ProposedDesignItems = new List<ConDesignItem>();
		}

		/// <summary>
		/// Gets or sets the unique identifier of the project being searched.
		/// </summary>
		public Guid ProjectId { get; set; }

		/// <summary>
		/// Gets or sets the identifier of the connection being searched.
		/// </summary>
		public int ConnectionId { get; set; }

		/// <summary>
		/// Gets or sets the search filter parameters for querying the connection library.
		/// </summary>
		public ConConnectionLibrarySearchParameters SearchParameters { get; set; }

		/// <summary>
		/// Gets or sets the list of design items proposed by the connection library search.
		/// </summary>
		public List<ConDesignItem> ProposedDesignItems { get; set; }

		/// <summary>
		/// Gets or sets the XML content of the selected connection template,
		/// or <see langword="null"/> if no template has been selected.
		/// </summary>
		public string? SelectedTemplateXml { get; set; }

		/// <summary>
		/// Gets or sets the raw picture data for the selected design item,
		/// or <see langword="null"/> if no picture is available.
		/// </summary>
		public byte[]? PictureData { get; set; }
	}
}
