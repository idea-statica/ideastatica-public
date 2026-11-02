using System;
using System.Xml.Serialization;

namespace IdeaRS.OpenModel
{
	/// <summary>
	/// Defines certain data about user project.
	/// </summary>
	[OpenModelClass("CI.ProjectData.ProjectData,CI.ProjectData", "CI.ProjectData.IProjectData,CI.BasicTypes")]
	public sealed class ProjectData : OpenObject
	{
		/// <summary>
		/// Gets or sets the name of project.
		/// </summary>
		[OpenModelProperty("ProjectName")]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the description of project.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the number of project.
		/// </summary>
		[OpenModelProperty("ProjectNo")]
		public string Number { get; set; }

		/// <summary>
		/// Gets or sets the project author.
		/// </summary>
		public string Author { get; set; }

		/// <summary>
		/// Gets or sets the date.
		/// </summary>
		[OpenModelProperty("DateOfCreate")]
		public DateTime Date { get; set; }

		/// <summary>
		/// Code dependent data
		/// </summary>
		[XmlElement(typeof(ProjectDataEc))]
		public object CodeDependentData { get; set; }
	}
}
