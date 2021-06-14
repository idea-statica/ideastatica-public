using System.Xml.Serialization;

namespace IdeaRS.OpenModel.Connection
{
	/// <summary>
	/// Represents settings of improt options into Idea Connections
	/// </summary>
	[XmlRootAttribute(ElementName = "IdeaConImportSettings", IsNullable = false)]
	public class IdeaConImportSettings
	{
		/// <summary>
		/// Constructor - it sets default values
		/// </summary>
		public IdeaConImportSettings()
		{
			UseWizard = true;
			DefaultBoltAssembly = "M12 4.6";
			DefaultWeldMaterial = string.Empty;
			DefaultSteelMaterial = string.Empty;
			ExportAllLinearCombination = true;
			OnePageWizard = true;
			CanChangeDesignCodeInWizard = true;
			WaitForExit = true;
			UserWizardBrand = string.Empty;
			DesignCode = "ECEN";
			StartIdeaStaticaApp = true;
			OrderMembersById = true;
			// DefaultConnectionFileName = null;
		}

		/// <summary>
		/// Open import wizard window
		/// </summary>
		public bool UseWizard { get; set; }

		/// <summary>
		/// If true, connected members in connection are ordered acoording to topological rules
		/// othewise by their IDs.
		/// </summary>
		public bool OrderMembersById { get; set; }

		/// <summary>
		/// Gets or sets the name of the default bolt assembly 
		/// </summary>
		public string DefaultBoltAssembly { get; set; }

		/// <summary>
		/// Gets or sets the name of the default material of plates which are added from the template
		/// </summary>
		public string DefaultSteelMaterial { get; set; }

		/// <summary>
		/// All linear combination will be exported to IDEA connection
		/// </summary>
		public bool ExportAllLinearCombination { get; set; }

		/// <summary>
		/// One page wizard
		/// </summary>
		public bool OnePageWizard { get; set; }

		/// <summary>
		/// Gets or sets the name of the Connection project
		/// </summary>
		public string ConnectionFileName { get; set; }

		/// <summary>
		/// WaitForExit
		/// </summary>
		public bool WaitForExit { get; set; }

		/// <summary>
		/// Can change design code in Wizard
		/// </summary>
		public bool CanChangeDesignCodeInWizard { get; set; }

		/// <summary>
		/// Set user Wizard Brand
		/// </summary>
		public string UserWizardBrand { get; set; }

		/// <summary>
		/// Running application
		/// </summary>
		public string RunningApplication { get; set; }

		/// <summary>
		/// Gets or sets the Design Code
		/// There can be :
		/// ECEN
		/// AISC
		/// CISC
		/// RUS
		/// </summary>
		public string DesignCode { get; set; }

		/// <summary>
		/// Gets or sets the name of the default weld material
		/// </summary>
		public string DefaultWeldMaterial { get; set; }

		/// <summary>
		/// StartIdeaStaticaApp
		/// </summary>
		public bool StartIdeaStaticaApp { get; set; }

		/// <summary>
		/// Name of the project
		/// </summary>
		public string ProjectName { get; set; }

		/// <summary>
		/// Description of the idea con project
		/// </summary>
		public string ProjectDescription { get; set; }

		/// <summary>
		/// The code which defines the source BIM application (the origin of the model)
		/// </summary>
		public string BimAppCode { get; set; }
	}
}
