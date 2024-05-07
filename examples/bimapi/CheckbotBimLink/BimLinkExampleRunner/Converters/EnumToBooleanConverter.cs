using System;
using System.Windows.Data;

namespace BimLinkExampleRunner.Converters
{
	[ValueConversion(typeof(Enum), typeof(bool))]
	public class EnumToBooleanConverter : IValueConverter
	{
		private static IValueConverter instance;

		public static IValueConverter Instance => instance ?? (instance = new EnumToBooleanConverter());

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (parameter is string s)
			{
				return value.Equals(Enum.Parse(value.GetType(), s, false));
			}
			else
			{
				return value.Equals(parameter);
			}
		}

		/// <summary>
		/// Converts a value.
		/// </summary>
		/// <param name="value">The value that is produced by the binding target.</param>
		/// <param name="targetType">The type to convert to.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value.Equals(false))
			{
				return Binding.DoNothing;
			}
			else
			{
				if (parameter is string s)
				{
					return Enum.Parse(targetType, s, false);
				}
				else
				{
					return parameter;
				}
			}
		}
	}

}