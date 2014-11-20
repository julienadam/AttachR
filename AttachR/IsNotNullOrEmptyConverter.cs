using System;
using System.Globalization;
using System.Windows.Data;

namespace AttachR
{
    public class IsNotNullOrEmptyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value != null) && (value as string) != "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("IsNotNullOrEmptyConverter can only be used OneWay.");
        }
    }
}