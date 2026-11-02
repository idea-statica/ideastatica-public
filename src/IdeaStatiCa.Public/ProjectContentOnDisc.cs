using System.Collections.Generic;
using System.IO;

namespace IdeaStatiCa.Public
{
	/// <summary>
	/// Summary description for Class1
	/// </summary>
	public class ProjectContentOnDisc : IProjectContent
	{
		private string WorkingDir { get; set; }
		public ProjectContentOnDisc(string workingDir)
		{
			Init(workingDir);
		}

		/// <summary>
		/// Initializes working directory.
		/// </summary>
		/// <param name="workingDir">Current working directory.</param>
		public void Init(string workingDir)
		{
			WorkingDir = workingDir;
		}

		/// <summary>
		/// Creates new filestream, if the file already exists, it will be overwritten.
		/// </summary>
		/// <param name="contentId">Name of the file to be created in the working directory.</param>
		/// <returns></returns>
		public Stream Create(string contentId)
		{
			string filePath = Path.Combine(WorkingDir, contentId);
			return new FileStream(filePath, FileMode.Create);
		}

		/// <summary>
		/// Deletes artefact with contentId identificatior.
		/// </summary>
		/// <param name="contentId">Uniqe artefact identificator.</param>
		public void Delete(string contentId)
		{
			string filePath = Path.Combine(WorkingDir, contentId);
			if (Directory.Exists(filePath))
			{
				Tools.DiskOperations.DeleteDirectory(filePath);
				return;
			}
			Tools.DiskOperations.DeleteFile(filePath);
		}

		/// <summary>
		/// Tells whether artefact with contentId identificatior exists or not.
		/// </summary>
		/// <param name="contentId">Uniqe artefact identificator.</param>
		/// <returns>Whether the artefact with contentId identificator exists or not.</returns>
		public bool Exist(string contentId)
		{
			string filePath = Path.Combine(WorkingDir, contentId);
			return Directory.Exists(filePath) || File.Exists(filePath);
		}

		/// <summary>
		/// Gets filestream from contentId file in working directory. If the file doesn't exist then new file is created.
		/// </summary>
		/// <param name="contentId">Uniqe artefact identificator.</param>
		/// <returns>Stream created from contentId.</returns>
		public Stream Get(string contentId)
		{
			string filePath = Path.Combine(WorkingDir, contentId);
			return new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);
		}


		/// <summary>
		/// Iterates through directories and subdirectories in order to find all paths included in traversal.
		/// </summary>
		/// <returns>Names of files (including their paths) in the working directory (including subdirectories).</returns>
		public List<ProjectDataItem> GetContent()
		{
			string[] entries = Directory.GetFileSystemEntries(WorkingDir, "*", SearchOption.AllDirectories);
			var result = new List<ProjectDataItem>();
			foreach (string dataItem in entries)
			{
				var relativePath = dataItem.Substring(WorkingDir.Length + 1);
				if (Directory.Exists(dataItem))
				{
					result.Add(new ProjectDataItem(relativePath, ItemType.Group));
				}
				else
				{
					result.Add(new ProjectDataItem(relativePath, ItemType.File));
				}
			}
			return result;
		}

		/// <summary>
		/// Copies content from one IProjectContent instance to another.
		/// </summary>
		/// <param name="projectContentSource">IProjectContent instance from which are data copied into the current one.</param>
		public void CopyContent(IProjectContent projectContentSource)
		{
			var sourceItems = projectContentSource.GetContent();
			foreach (ProjectDataItem dataItem in sourceItems)
			{
				if (dataItem.Type.Equals(ItemType.Group))
				{
					Directory.CreateDirectory(Path.Combine(WorkingDir, dataItem.Name));
				}
			}

			foreach (ProjectDataItem dataItem in sourceItems)
			{
				if (dataItem.Type.Equals(ItemType.File))
				{
					using (Stream dataSourceStream = projectContentSource.Get(dataItem.Name))
					{
						var destFileName = Path.Combine(WorkingDir, dataItem.Name);
						using (FileStream dataDestStream = new FileStream(destFileName, FileMode.Create, FileAccess.Write))
						{
							dataSourceStream.CopyTo(dataDestStream);
						}
					}
				}
			}
		}
	}
}
