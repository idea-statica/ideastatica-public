using System.Collections.Generic;
using System.IO;

namespace IdeaStatiCa.Public
{
	/// <summary>
	/// Provides methods for accessing project data streams.
	/// </summary>
	public interface IProjectContent
	{
		/// <summary>
		/// Creates new project data stream. If <paramref name="contentId"/> already exists, it will be overwritten.
		/// </summary>
		/// <param name="contentId">Uniqe identifier of stream.</param>
		/// <returns></returns>
		Stream Create(string contentId);

		/// <summary>
		/// Loads data from artefact into stream.
		/// </summary>
		/// <param name="contentId">Unique artefact identificator.</param>
		/// <returns>Newly created stream.</returns>
		Stream Get(string contentId);

		/// <summary>
		/// Tells whether artefact with <paramref name="contentId"/> identificatior exists or not.
		/// </summary>
		/// <param name="contentId">Uniqe artefact identificator.</param>
		/// <returns>Whether the artefact with contentId identificator exists or not.</returns>
		bool Exist(string contentId);

		/// <summary>
		/// Deletes artefact with <paramref name="contentId"/> identificatior.
		/// </summary>
		/// <param name="contentId">Uniqe artefact identificator.</param>
		void Delete(string contentId);

		/// <summary>
		/// Iterates through directories and subdirectories in order to find all paths included in traversal.
		/// </summary>
		/// <returns>Names of files (including their paths) in the working directory (including subdirectories).</returns>
		List<ProjectDataItem> GetContent();

		/// <summary>
		/// Copies <paramref name="sourceContent"/> .
		/// </summary>
		/// <param name="sourceContent">Source data</param>
		void CopyContent(IProjectContent sourceContent);
	}

	/// <summary>
	/// Represents one project data item.
	/// </summary>

	public class ProjectDataItem
	{
		public string Name { get; set; }
		public ItemType Type { get; set; }

		public ProjectDataItem(string name, ItemType type)
		{
			Name = name;
			Type = type;
		}

		public override bool Equals(object obj)
		{
			ProjectDataItem o = obj as ProjectDataItem;
			if (obj == null)
				return false;
			return (Name.Equals(o.Name) && Type.Equals(o.Type));
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode() + Type.GetHashCode();
		}
	}

	/// <summary>
	/// Represents possible types of project data item.
	/// </summary>
	public enum ItemType
	{
		File,
		Group
	}
}
