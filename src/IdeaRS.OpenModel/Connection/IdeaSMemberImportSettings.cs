using System.Xml.Serialization;

namespace IdeaRS.OpenModel.Connection
{

	/// <summary>
	/// Represents settings of improt options into Idea Connections
	/// </summary>
	[XmlRootAttribute(ElementName = "IdeaSMemberImportSettings", IsNullable = false)]
	public class IdeaSMemberImportSettings : IdeaConImportSettings
	{
		/// <summary>
		/// Constructor - it sets default values
		/// </summary>
		public IdeaSMemberImportSettings() : base()
		{
			// DefaultConnectionFileName = null;
		}

	}
}
