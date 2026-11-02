namespace IdeaStatiCa.CheckbotPlugin.Models
{
	public class ModelExportOptions
	{
		public static readonly ModelExportOptions Default = new ModelExportOptions();

		/// <summary>
		/// When <see langword="true"/> OpenModelContainer will contains results.
		/// </summary>
		public bool WithResults { get; set; } = true;

		/// <remarks>Not implemented yet</remarks>
		public bool AllCrossSectionsAsGeneral { get; set; } = false;

		/// <summary>
		/// By Version is possible do downgrade of OpenModelContainer
		/// </summary>
		public string Version { get; set; } = "";
	}
}