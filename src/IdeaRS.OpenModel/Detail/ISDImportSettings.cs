using System.Xml.Serialization;

namespace IdeaRS.OpenModel.Detail
{
	/// <summary>
	/// Represents settings of improt options into Idea StatiCa Detail
	/// </summary>
	[XmlRootAttribute(ElementName = "ISDImportSettings", IsNullable = false)]
	public class ISDImportSettings
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ISDImportSettings()
		{
			UseWizard = true;
			IsUpdate = false;
			UseSync = false;
		}

		/// <summary>
		/// Open import wizard window
		/// </summary>
		public bool UseWizard { get; set; }

		public bool IsUpdate { get; set; }

		/// <summary>
		/// Enable synchronization data tracking (SyncId and SyncData file).
		/// When false, no synchronization file will be created and SyncIds will not be used.
		/// </summary>
		public bool UseSync { get; set; }
	}
}