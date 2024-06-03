using System;

namespace IdeaStatiCa.Plugin.Api.ConnectionRest.Model.Model_Project
{
	public class ConProjectData
	{
		/// <summary>
		/// The name of the project
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The description of the project
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Project number
		/// </summary>
		public string ProjectNumber { get; set; }

		/// <summary>
		/// Name of the author
		/// </summary>
		public string Author { get; set; }

		/// <summary>
		/// Design code
		/// </summary>
		public string DesignCode { get; set; }

		/// <summary>
		/// Date
		/// </summary>
		public DateTime Date { get; set; }
	}
}
