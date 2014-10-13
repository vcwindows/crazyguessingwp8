using System;
using System.Windows.Data;
using System.Windows.Media;

namespace CrazyGuessing.Converters
{
    public class ItemForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value ? new SolidColorBrush(Color.FromArgb(0xff, 0x00, 0xcc, 0x00)) : new SolidColorBrush(Color.FromArgb(0xff, 0xff, 0x33, 0x00));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
