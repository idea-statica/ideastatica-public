﻿namespace IdeaStatiCa.Plugin.Api.RCS.Model
{
	/// <summary>
	/// A setting of an import of a reinforced css template
	/// </summary>
	public class ReinfCssImportSetting
	{
		public ReinfCssImportSetting()
		{
			PartsToImport = "Complete";
		}

		/// <summary>
		/// Id of a reinforced cross-section in the active RCS project.
		/// If a value is not provided a new reinforced cross-section will be created in the RCS project.
		/// </summary>
		public int? ReinfCssId { get; set; }

		/// <summary>
		/// Defines what to import from a NAV file. Values can be 'Complete', 'Css', 'Reinf' or 'Tendon'
		/// </summary>
		public string PartsToImport { get; set; }
	}
}
