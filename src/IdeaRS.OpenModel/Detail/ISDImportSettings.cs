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
		}

		/// <summary>
		/// Open import wizard window
		/// </summary>
		public bool UseWizard { get; set; }
	}
}