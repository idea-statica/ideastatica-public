using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

// The FOLDER is Converters/ but the NAMESPACE stays NorsokChecker.Controls, matching
// FractionConverters.cs beside it: MainWindow.xaml reaches these through
// xmlns:controls="clr-namespace:NorsokChecker.Controls", and a namespace that does not match fails
// at RUNTIME as a missing StaticResource rather than at compile time.
namespace NorsokChecker.Controls
{
	/// <summary>
	/// Turns a column header like <c>"M_y,chord [kNm]"</c> into typeset inlines —
	/// <c>M</c> with <c>y,chord</c> as a real subscript, then the unit.
	///
	/// Two problems this solves at once, both found on a rendered §6.4 tab:
	///
	/// - **WPF EATS the underscore.** A DataGridColumn header is content for a ContentPresenter,
	///   which treats <c>_</c> as an access-key marker, so <c>"N_Sd [kN]"</c> displayed as
	///   <c>"NSd [kN]"</c>. The classification table below kept its underscores only because it has
	///   a HeaderStyle whose ContentTemplate routes the string through a TextBlock — an accident of
	///   the group-banner work, not a decision. One table showed <c>My</c> and the other
	///   <c>M_y,Rd</c>.
	/// - **An underscore is not notation.** These are physical symbols; the subscript belongs below
	///   the baseline, as the report already types it.
	///
	/// Written as a converter rather than per-column inlines because there are a dozen headers
	/// across two tables: spelling each one out in XAML would be five lines apiece and would drift
	/// the moment someone adds a column.
	/// </summary>
	public sealed class SubscriptHeaderConverter : IValueConverter
	{
		/// <summary>
		/// <c>SYMBOL_SUBSCRIPT</c> optionally followed by a unit in brackets. The subscript runs to
		/// the first space or bracket, so <c>"M_y,chord [kNm]"</c> splits as M / y,chord / " [kNm]" —
		/// a comma is part of a subscript here (<c>y,chord</c>, <c>y,Rd</c>), a space ends it.
		/// </summary>
		private static readonly Regex Pattern = new(@"^([^_\s]+)_([^\s\[]+)(.*)$", RegexOptions.Compiled);

		public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			string text = value?.ToString() ?? "";

			var block = new TextBlock
			{
				TextWrapping = TextWrapping.NoWrap,
				TextAlignment = TextAlignment.Center,
			};

			var m = Pattern.Match(text);
			if (!m.Success)
			{
				// No subscript to typeset — but still go through a TextBlock, or the underscore in a
				// header this pattern does not match would be swallowed as an access key.
				block.Text = text;
				return block;
			}

			block.Inlines.Add(new Run(m.Groups[1].Value));
			block.Inlines.Add(new Run(m.Groups[2].Value)
			{
				BaselineAlignment = BaselineAlignment.Subscript,
				FontSize = 9.0,
			});
			if (m.Groups[3].Value.Length > 0)
				block.Inlines.Add(new Run(m.Groups[3].Value));

			return block;
		}

		public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
			throw new NotSupportedException();
	}
}
