using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NorsokChecker.Controls
{
	/// <summary>
	/// A 0..1 fraction as a star GridLength, for drawing a proportional bar.
	///
	/// WPF has no percentage width, so a bar that fills part of a row is done as a two-column grid
	/// whose star widths are the fraction and its complement. Both halves have to come from the same
	/// number, hence the pair of converters.
	///
	/// Zero maps to an ABSOLUTE zero rather than 0*: a 0* column keeps a minimum size in some
	/// layouts, so an unloaded state would still show a sliver of colour.
	/// </summary>
	public sealed class FractionToStarConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			double f = Clamp(value);
			return f <= 0 ? new GridLength(0) : new GridLength(f, GridUnitType.Star);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			=> throw new NotSupportedException();

		internal static double Clamp(object value) =>
			value is double d && !double.IsNaN(d) ? Math.Clamp(d, 0.0, 1.0) : 0.0;
	}

	/// <summary>The remainder of <see cref="FractionToStarConverter"/> — the unfilled part of the bar.</summary>
	public sealed class FractionToRestStarConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			double rest = 1.0 - FractionToStarConverter.Clamp(value);
			return rest <= 0 ? new GridLength(0) : new GridLength(rest, GridUnitType.Star);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			=> throw new NotSupportedException();
	}
}
