namespace NorsokChecker.Models
{
	/// <summary>
	/// Display model for the Members DataGrid — auto-populated from API.
	/// No buckling parameters here: L, k and the far-end moments are properties of the member's
	/// unbraced SPAN, which lies outside the joint this app knows — see CHAPTER_63_FINDINGS.md.
	/// They were once editable columns, and the run silently overwrote whatever was typed.
	/// </summary>
	public class MemberDisplayInfo
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		/// <summary>"Chord" or "Brace"</summary>
		public string Role { get; set; } = string.Empty;
		/// <summary>"CHS", "RHS", "I-section", "Channel", "Angle", "Other"</summary>
		public string Shape { get; set; } = string.Empty;
		/// <summary>Profile description e.g. "CHS 500/20", "HEB 300"</summary>
		public string Profile { get; set; } = string.Empty;
		/// <summary>CHS: outside diameter [mm]. I: height h [mm].</summary>
		public double Diameter { get; set; }
		/// <summary>CHS: wall thickness [mm]. I: web thickness tw [mm].</summary>
		public double WallThickness { get; set; }
		public double Fy { get; set; } = 355;
		public string MaterialName { get; set; } = string.Empty;
		public bool IsCHS => Shape == "CHS";

		/// <summary>
		/// The three numeric columns, ALREADY FORMATTED in the user's units.
		///
		/// The grid used to bind straight to the numbers with `StringFormat=F1` and carry the unit
		/// in the header text — `Header="D [mm]"`. That is why changing the unit setting did nothing
		/// to this table: a StringFormat in XAML cannot see a setting, and a header written in the
		/// markup cannot change with one.
		///
		/// So the model formats, and the header is filled from code. Setting these is
		/// <see cref="ApplyDisplay"/>'s job, called whenever the table is built or the setting
		/// changes.
		/// </summary>
		public string DiameterText { get; private set; } = "";
		public string ThicknessText { get; private set; } = "";
		public string FyText { get; private set; } = "";

		/// <summary>
		/// Render the numbers for the current settings.
		///
		/// Diameter, WallThickness and Fy are held here in MILLIMETRES and MPa, not SI like the rest
		/// of the app — this model is filled straight from the API's member list. They are converted
		/// back to SI first so there is one conversion path rather than a second set of factors.
		/// </summary>
		public void ApplyDisplay(DisplaySettings s, System.Globalization.CultureInfo c)
		{
			DiameterText = Diameter > 0 ? QuantityFormat.Length(Diameter / 1e3, c, s) : "";
			ThicknessText = WallThickness > 0 ? QuantityFormat.SmallLength(WallThickness / 1e3, c, s) : "";
			FyText = Fy > 0 ? QuantityFormat.Stress(Fy * 1e6, c, s) : "";
		}

	}
}
