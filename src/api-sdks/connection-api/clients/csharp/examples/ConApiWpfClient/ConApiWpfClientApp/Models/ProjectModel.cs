using CommunityToolkit.Mvvm.ComponentModel;
using IdeaStatiCa.Api.Connection.Model;
using System;

namespace ConApiWpfClientApp.Models
{
	/// <summary>
	/// Model representing the currently open project data.
	/// </summary>
	public partial class ProjectModel : ObservableObject
	{
		/// <summary>
		/// Gets or sets the currently open project information, or <see langword="null"/> if no project is open.
		/// </summary>
		[ObservableProperty]
		private ConProject? _projectInfo;

		/// <summary>
		/// Gets the project identifier, or <see cref="Guid.Empty"/> if no project is open.
		/// </summary>
		public Guid ProjectId => ProjectInfo?.ProjectId ?? Guid.Empty;

		/// <summary>
		/// Resets the model to its initial state (no project open).
		/// </summary>
		public void Clear()
		{
			ProjectInfo = null;
		}
	}
}
