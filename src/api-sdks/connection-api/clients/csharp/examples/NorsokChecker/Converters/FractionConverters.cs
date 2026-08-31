using System.Globalization;
using System.Windows;
using System.Windows.Data;

// The FOLDER is Converters/ but the NAMESPACE stays NorsokChecker.Controls, deliberately: MainWindow
// .xaml reaches these through xmlns:controls="clr-namespace:NorsokChecker.Controls", and a namespace
// change there fails at RUNTIME as a missing StaticResource, not at compile time. C# namespaces do
// not have to follow folders, so the file sits where it belongs at no risk. Rename the namespace
// only together with the XAML, and only when something else already forces that file open.
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

	/// <summary>
	/// A Color to a Brush, for binding a row's Background to a computed colour.
	///
	/// Needed because Background wants a Brush and the row exposes a Color — the utilisation tint is
	/// computed per row, so it cannot be a static resource. Transparent maps to null so the row keeps
	/// whatever the theme gives it (alternating stripes, selection) rather than being painted over
	/// with a transparent brush that defeats them.
	/// </summary>
	public sealed class ColorToBrushConverter : IValueConverter
	{
		public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is not System.Windows.Media.Color c) return null;
			if (c.A == 0) return null;
			var brush = new System.Windows.Media.SolidColorBrush(c);
			brush.Freeze();
			return brush;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			=> throw new NotSupportedException();
	}
}
