using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Jellyfish.Library
{
    public class StringFormatConverter : IValueConverter // SL is missing Binding.StringFormat
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string))
            {
                return DependencyProperty.UnsetValue;
            }

            if (value == null)
            {
                return string.Empty;
            }

            string format = parameter as string;
            if (!string.IsNullOrEmpty(format))
            {
                if (format.IndexOf('{') < 0)
                {
                    format = "{0:" + format + "}";
                }

                return string.Format(culture, format, value);
            }

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue; // one way only
        }
    }
}
