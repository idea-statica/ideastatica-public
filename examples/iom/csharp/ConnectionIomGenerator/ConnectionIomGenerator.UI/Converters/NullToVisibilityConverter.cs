using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ConnectionIomGenerator.UI.Converters
{
	/// <summary>
	/// Converts null or empty string to Visibility.Collapsed, otherwise to Visibility.Visible.
	/// </summary>
	public class NullToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
				return Visibility.Collapsed;

			if (value is string str && string.IsNullOrWhiteSpace(str))
				return Visibility.Collapsed;

			return Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
