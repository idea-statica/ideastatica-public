using System.Xml.Serialization;

namespace IdeaRS.OpenModel.Beam
{
	/// <summary>
	/// Represents settings of improt options into Idea Connections
	/// </summary>
	[XmlRootAttribute(ElementName = "IdeaBeamImportSettings", IsNullable = false)]
	public class IdeaBeamImportSettings
	{
		/// <summary>
		/// Constructor - it sets default values
		/// </summary>
		public IdeaBeamImportSettings()
		{
			UseWizard = true;
			ExportAllLinearCombination = true;
			UserWizardBrand = string.Empty;
			DesignCode = "ECEN";
			CanChangeDesignCodeInWizard = true;
		}

		/// <summary>
		/// Open import wizard window
		/// </summary>
		public bool UseWizard { get; set; }

		/// <summary>
		/// All linear combination will be exported to IDEA connection
		/// </summary>
		public bool ExportAllLinearCombination { get; set; }

		/// <summary>
		/// Set user Wizard Brand
		/// </summary>
		public string UserWizardBrand { get; set; }

		/// <summary>
		/// Gets or sets the Design Code
		/// </summary>
		public string DesignCode { get; set; }

		/// <summary>
		/// Can change design code in Wizard
		/// </summary>
		public bool CanChangeDesignCodeInWizard { get; set; }

		/// <summary>
		/// Gets or sets the name of the Beam project
		/// </summary>
		public string BeamFileName { get; set; }
	}
}
