namespace IdeaStatiCa.CheckbotPlugin.Models
{
	public class ModelExportOptions
	{
		public static readonly ModelExportOptions Default = new ModelExportOptions();

		/// <summary>
		/// When <see langword="true"/> <see cref="IdeaRS.OpenModel.OpenModelContainer"/> will contains results.
		/// </summary>
		public bool WithResults { get; set; } = true;

		/// <summary>
		/// When <see langword="true"/> all cross-section in the IOM will be specified as <see cref="IdeaRS.OpenModel.CrossSection.CrossSectionComponent"/>.
		/// </summary>
		public bool AllCrossSectionsAsGeneral { get; set; } = false;
	}
}